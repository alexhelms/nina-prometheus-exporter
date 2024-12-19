using AlexHelms.NINA.PrometheusExporter;
using NINA.Astrometry;
using NINA.Profile;
using NINA.Profile.Interfaces;
using Prometheus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlexHelms.NINA.Prometheusexporter.NinaMetrics;

public class OtherMetrics : IDisposable
{
    private readonly IProfileService _profileService;
    private readonly PrometheusExporterOptions _options;

    public static readonly Gauge SunAltitude = Metrics.CreateGauge("nina_sun_altitude", "Sun altitude in degrees.");
    public static readonly Gauge MoonAltitude = Metrics.CreateGauge("nina_moon_altitude", "Moon altitude in degrees.");
    public static readonly Gauge MoonIllumination = Metrics.CreateGauge("nina_moon_illumination", "Moon illuminationm, 0 to 100%.");
    public static readonly Gauge MoonPositionAngle = Metrics.CreateGauge("nina_moon_position_angle", "Moon position angle in degrees.");

    private PeriodicTimer _timer;

    public OtherMetrics(IProfileService profileService, PrometheusExporterOptions options)
    {
        _profileService = profileService;
        _options = options;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _timer?.Dispose();
        UnpublishMetrics();
    }

    public async Task Run(CancellationToken token)
    {
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        try
        {
            PublishMetrics();

            while (await _timer.WaitForNextTickAsync(token))
            {
                PublishMetrics();
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            UnpublishMetrics();
        }
    }

    private void PublishMetrics()
    {
        if (_options.EnableOtherMetrics)
        {
            var observerInfo = new ObserverInfo
            {
                Latitude = _profileService.ActiveProfile.AstrometrySettings.Latitude,
                Longitude = _profileService.ActiveProfile.AstrometrySettings.Longitude,
                Elevation = _profileService.ActiveProfile.AstrometrySettings.Elevation,
            };
            var utcNow = DateTime.UtcNow;
            var sunAltitude = AstroUtil.GetSunAltitude(utcNow, observerInfo);
            var moonAltitude = AstroUtil.GetMoonAltitude(utcNow, observerInfo);
            var moonIllumination = AstroUtil.GetMoonIllumination(utcNow) * 100.0;
            var moonPositionAngle = AstroUtil.GetMoonPositionAngle(utcNow);

            SunAltitude.Set(sunAltitude);
            MoonAltitude.Set(moonAltitude);
            MoonIllumination.Set(moonIllumination);
            MoonPositionAngle.Set(moonPositionAngle);
        }
        else
        {
            UnpublishMetrics();
        }
    }

    private void UnpublishMetrics()
    {
        SunAltitude.Unpublish();
        MoonAltitude.Unpublish();
        MoonIllumination.Unpublish();
        MoonPositionAngle.Unpublish();
    }
}
