using AlexHelms.NINA.PrometheusExporter;
using NINA.Equipment.Equipment.MyFocuser;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;
using System;

namespace AlexHelms.NINA.Prometheusexporter.NinaMetrics;

public class FocuserMetrics : IFocuserConsumer
{
    private readonly IFocuserMediator _focuser;
    private readonly PrometheusExporterOptions _options;

    private static readonly string[] Labels = ["focuser_name"];
    private static readonly Gauge Temperature = Metrics.CreateGauge("nina_focuser_temperature", "Focuser temperature in degrees C.", Labels);
    private static readonly Gauge Position = Metrics.CreateGauge("nina_focuser_position", "Focuser position in steps.", Labels);
    private static readonly Counter Autofocus = Metrics.CreateCounter("nina_autofocus", "Number of autofocus runs.", Labels);

    private string _name = string.Empty;

    public FocuserMetrics(IFocuserMediator focuser, PrometheusExporterOptions options)
    {
        _options = options;
        _focuser = focuser;
        _focuser.RegisterConsumer(this);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _focuser.RemoveConsumer(this);
    }

    public void UpdateDeviceInfo(FocuserInfo deviceInfo)
    {
        _name = deviceInfo.Name ?? "none";
        var labels = new[] { _name };

        if (_options.EnableFocuserMetrics && deviceInfo.Connected)
        {
            Temperature.WithLabels(labels).Set(Util.ReplaceNan(deviceInfo.Temperature));
            Position.WithLabels(labels).Set(Util.ReplaceNan(deviceInfo.Position));
        }
        else
        {
            Temperature.WithLabels(labels).Unpublish();
            Position.WithLabels(labels).Unpublish();
        }
    }

    public void UpdateEndAutoFocusRun(AutoFocusInfo info)
    {
        var labels = new[] { _name };

        if (_options.EnableFocuserMetrics)
        {
            Autofocus.WithLabels(labels).Inc();
        }
        else
        {
            Autofocus.WithLabels(labels).Unpublish();
        }
    }

    public void UpdateUserFocused(FocuserInfo info) { }
}
