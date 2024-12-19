using NINA.Core.Utility;
using NINA.Profile;
using NINA.Profile.Interfaces;
using System;
using Settings = AlexHelms.NINA.PrometheusExporter.Properties.Settings;

namespace AlexHelms.NINA.PrometheusExporter;

public class PrometheusExporterOptions : BaseINPC, IDisposable
{
    private readonly IProfileService _profileService;
    private readonly PluginOptionsAccessor _options;

    public PrometheusExporterOptions(IProfileService profileService)
    {
        _profileService = profileService;
        _profileService.ProfileChanged += ProfileService_ProfileChanged;
        _profileService.ActiveProfile.PluginSettings.PropertyChanged += ProfileService_ProfileChanged;

        var guid = PluginOptionsAccessor.GetAssemblyGuid(typeof(PrometheusExporter));
        _options = new PluginOptionsAccessor(profileService, guid.Value);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _profileService.ProfileChanged -= ProfileService_ProfileChanged;
        _profileService.ActiveProfile.PluginSettings.PropertyChanged -= ProfileService_ProfileChanged;
    }

    private void ProfileService_ProfileChanged(object sender, EventArgs e)
    {
        RaiseAllPropertiesChanged();
    }

    public static string Hostname { get; } = Environment.MachineName.Replace('-', '_');

    public string ProfileId => _profileService.ActiveProfile?.Id.ToString() ?? Guid.Empty.ToString();

    public int Port
    {
        get => _options.GetValueInt32(nameof(Port), 9100);
        set
        {
            _options.SetValueInt32(nameof(Port), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }

    public bool EnableCameraMetrics
    {
        get => _options.GetValueBoolean(nameof(EnableCameraMetrics), true);
        set
        {
            _options.SetValueBoolean(nameof(EnableCameraMetrics), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }

    public bool EnableFocuserMetrics
    {
        get => _options.GetValueBoolean(nameof(EnableFocuserMetrics), true);
        set
        {
            _options.SetValueBoolean(nameof(EnableFocuserMetrics), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }

    public bool EnableGuiderMetrics
    {
        get => _options.GetValueBoolean(nameof(EnableGuiderMetrics), true);
        set
        {
            _options.SetValueBoolean(nameof(EnableGuiderMetrics), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }

    public bool EnableImageMetadataMetrics
    {
        get => _options.GetValueBoolean(nameof(EnableImageMetadataMetrics), true);
        set
        {
            _options.SetValueBoolean(nameof(EnableImageMetadataMetrics), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }

    public bool EnableMountMetrics
    {
        get => _options.GetValueBoolean(nameof(EnableMountMetrics), true);
        set
        {
            _options.SetValueBoolean(nameof(EnableMountMetrics), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }

    public bool EnableOtherMetrics
    {
        get => _options.GetValueBoolean(nameof(EnableOtherMetrics), true);
        set
        {
            _options.SetValueBoolean(nameof(EnableOtherMetrics), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }

    public bool EnableRotatorMetrics
    {
        get => _options.GetValueBoolean(nameof(EnableRotatorMetrics), true);
        set
        {
            _options.SetValueBoolean(nameof(EnableRotatorMetrics), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }

    public bool EnableSafetyMetrics
    {
        get => _options.GetValueBoolean(nameof(EnableSafetyMetrics), true);
        set
        {
            _options.SetValueBoolean(nameof(EnableSafetyMetrics), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }

    public bool EnableWeatherMetrics
    {
        get => _options.GetValueBoolean(nameof(EnableWeatherMetrics), true);
        set
        {
            _options.SetValueBoolean(nameof(EnableWeatherMetrics), value);
            CoreUtil.SaveSettings(Settings.Default);
            RaisePropertyChanged();
        }
    }
}
