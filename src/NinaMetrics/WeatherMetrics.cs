using AlexHelms.NINA.PrometheusExporter;
using NINA.Equipment.Equipment.MyWeatherData;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;
using System;

namespace AlexHelms.NINA.Prometheusexporter.NinaMetrics;

public class WeatherMetrics : IWeatherDataConsumer
{
    private readonly IWeatherDataMediator _weather;
    private readonly PrometheusExporterOptions _options;

    private static readonly Gauge CloudCover = Metrics.CreateGauge("nina_weather_cloud_cover", "Weather cloud cover, 0 to 100%.");
    private static readonly Gauge DewPoint = Metrics.CreateGauge("nina_weather_dew_point", "Weather dew point in degrees C.");
    private static readonly Gauge Humidity = Metrics.CreateGauge("nina_weather_humidity", "Weather humidity, 0 to 100%.");
    private static readonly Gauge Pressure = Metrics.CreateGauge("nina_weather_pressure", "Weather pressure in hPa.");
    private static readonly Gauge RainRate = Metrics.CreateGauge("nina_weather_rain_rate", "Weather rain rate in mm/hr.");
    private static readonly Gauge SkyBrightness = Metrics.CreateGauge("nina_weather_sky_brightness", "Weather sky brightness in lux.");
    private static readonly Gauge SkyQuality = Metrics.CreateGauge("nina_weather_sky_quality", "Weather sky quality in mag/sq-arcsec.");
    private static readonly Gauge SkyTemperature = Metrics.CreateGauge("nina_weather_sky_temperature", "Weather sky temperature in degrees C.");
    private static readonly Gauge StarFwhm = Metrics.CreateGauge("nina_weather_star_fwhm", "Weather star FWHM in arcsec.");
    private static readonly Gauge Temperature = Metrics.CreateGauge("nina_weather_temperature", "Weather temperature in degrees C.");
    private static readonly Gauge WindDirection = Metrics.CreateGauge("nina_weather_wind_direction", "Weather wind direction in degrees.");
    private static readonly Gauge WindGust = Metrics.CreateGauge("nina_weather_wind_gust", "Weather wind gust in m/s.");
    private static readonly Gauge WindSpeed = Metrics.CreateGauge("nina_weather_wind_speed", "Weather wind speed m/s.");

    public WeatherMetrics(IWeatherDataMediator weather, PrometheusExporterOptions options)
    {
        _weather = weather;
        _options = options;
        _weather.RegisterConsumer(this);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _weather.RemoveConsumer(this);
    }

    public void UpdateDeviceInfo(WeatherDataInfo deviceInfo)
    {
        if (_options.EnableWeatherMetrics && deviceInfo.Connected)
        {
            CloudCover.Set(Util.ReplaceNan(deviceInfo.CloudCover));
            DewPoint.Set(Util.ReplaceNan(deviceInfo.DewPoint));
            Humidity.Set(Util.ReplaceNan(deviceInfo.Humidity));
            Pressure.Set(Util.ReplaceNan(deviceInfo.Pressure));
            RainRate.Set(Util.ReplaceNan(deviceInfo.RainRate));
            SkyBrightness.Set(Util.ReplaceNan(deviceInfo.SkyBrightness));
            SkyQuality.Set(Util.ReplaceNan(deviceInfo.SkyQuality));
            SkyTemperature.Set(Util.ReplaceNan(deviceInfo.SkyTemperature));
            StarFwhm.Set(Util.ReplaceNan(deviceInfo.StarFWHM));
            Temperature.Set(Util.ReplaceNan(deviceInfo.Temperature));
            WindDirection.Set(Util.ReplaceNan(deviceInfo.WindDirection));
            WindGust.Set(Util.ReplaceNan(deviceInfo.WindGust));
            WindSpeed.Set(Util.ReplaceNan(deviceInfo.WindSpeed));
        }
        else
        {
            CloudCover.Unpublish();
            DewPoint.Unpublish();
            Humidity.Unpublish();
            Pressure.Unpublish();
            RainRate.Unpublish();
            SkyBrightness.Unpublish();
            SkyQuality.Unpublish();
            SkyTemperature.Unpublish();
            StarFwhm.Unpublish();
            Temperature.Unpublish();
            WindDirection.Unpublish();
            WindGust.Unpublish();
            WindSpeed.Unpublish();
        }
    }
}
