using NINA.Equipment.Equipment.MyCamera;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;

namespace AlexHelms.NINA.PrometheusExporter;

public class CameraMetrics : ICameraConsumer
{
    private readonly ICameraMediator _camera;
    private readonly PrometheusExporterOptions _options;

    private static readonly string[] Labels = new[] { "camera_name" };
    private readonly Gauge SensorTemperature = Metrics.CreateGauge("nina_camera_sensor_temperature", "Camera sensor temperature in degrees C.", Labels);
    private readonly Gauge SensorCoolerPower = Metrics.CreateGauge("nina_camera_cooler_power", "Camera cooler power, 0 to 100%.", Labels);
    private readonly Gauge BatteryLevel = Metrics.CreateGauge("nina_camera_battery_level", "Camera battery level, 0 to 100%.", Labels);

    public CameraMetrics(ICameraMediator camera, PrometheusExporterOptions options)
    {
        _camera = camera;
        _options = options;
        _camera.RegisterConsumer(this);
    }

    public void Dispose()
    {
        _camera.RemoveConsumer(this);
    }

    public void UpdateDeviceInfo(CameraInfo deviceInfo)
    {
        if (!deviceInfo.Connected)
            return;

        var labels = new[] { deviceInfo.Name ?? "none" };

        if (_options.EnableCameraMetrics)
        {
            SensorTemperature.WithLabels(labels).Set(deviceInfo.Temperature);
            SensorCoolerPower.WithLabels(labels).Set(deviceInfo.CoolerPower);
            BatteryLevel.WithLabels(labels).Set(deviceInfo.Battery);
        }
        else
        {
            SensorTemperature.WithLabels(labels).Unpublish();
            SensorCoolerPower.WithLabels(labels).Unpublish();
            BatteryLevel.WithLabels(labels).Unpublish();
        }
    }
}
