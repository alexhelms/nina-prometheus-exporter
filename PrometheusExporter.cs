using NINA.Core.Utility;
using NINA.Core.Utility.Notification;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.Interfaces.Mediator;
using Prometheus;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Settings = AlexHelms.NINA.PrometheusExporter.Properties.Settings;

namespace AlexHelms.NINA.PrometheusExporter;

[Export(typeof(IPluginManifest))]
public class PrometheusExporter : PluginBase
{
    private readonly MetricServer _server;
    private readonly CameraMetrics _cameraMetrics;
    private readonly FocuserMetrics _focuserMetrics;
    private readonly GuiderMetrics _guiderMetrics;
    private readonly ImageMetadataMetrics _imageMetadataMetrics;
    private readonly MountMetrics _mountMetrics;
    private readonly RotatorMetrics _rotatorMetrics;
    private readonly SafetyMetrics _safetyMetrics;
    private readonly WeatherMetrics _weatherMetrics;

    public PrometheusExporterOptions Options { get; }

    [ImportingConstructor]
    public PrometheusExporter(
        IProfileService profileService,
        ICameraMediator camera,
        IGuiderMediator guider,
        IFocuserMediator focuser,
        IImageSaveMediator images,
        ITelescopeMediator mount,
        IRotatorMediator rotator,
        ISafetyMonitorMediator safety,
        IWeatherDataMediator weather)
    {
        if (Settings.Default.UpdateSettings)
        {
            Settings.Default.Upgrade();
            Settings.Default.UpdateSettings = false;
            CoreUtil.SaveSettings(Settings.Default);
        }

        Options = new PrometheusExporterOptions(profileService);

        // Static metrics must be defined before any others.
        Metrics.SuppressDefaultMetrics();
        Metrics.DefaultRegistry.SetStaticLabels(new Dictionary<string, string>
        {
            ["hostname"] = PrometheusExporterOptions.Hostname,
            ["profile"] = Options.ProfileId,
        });

        _server = new MetricServer(Options.Port);
        _cameraMetrics = new CameraMetrics(camera, Options);
        _focuserMetrics = new FocuserMetrics(focuser, Options);
        _guiderMetrics = new GuiderMetrics(guider, Options);
        _imageMetadataMetrics = new ImageMetadataMetrics(images, Options);
        _mountMetrics = new MountMetrics(mount, Options);
        _rotatorMetrics = new RotatorMetrics(rotator, Options);
        _safetyMetrics = new SafetyMetrics(safety, Options);
        _weatherMetrics = new WeatherMetrics(weather, Options);
    }

    public override Task Initialize()
    {
        try
        {
            _server.Start();
        }
        catch (Exception e)
        {
            if (e.Message.Contains("Access is denied"))
            {
                Notification.ShowError($"Cannot start metrics server unless run as administrator or" +
                    $"You add an exception for the metrics server. See plugin documentation.");
            }
            else
            {
                Notification.ShowError($"Failed to start prometheus exporter server: {e.Message}");
            }
        }

        return base.Initialize();
    }

    public override Task Teardown()
    {
        _server.Dispose();
        _cameraMetrics.Dispose();
        _focuserMetrics.Dispose();
        _guiderMetrics.Dispose();
        _imageMetadataMetrics.Dispose();
        _mountMetrics.Dispose();
        _rotatorMetrics.Dispose();
        _safetyMetrics.Dispose();
        _weatherMetrics.Dispose();
        Options.Dispose();

        return base.Teardown();
    }
}
