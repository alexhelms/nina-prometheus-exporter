using AlexHelms.NINA.PrometheusExporter;
using NINA.Equipment.Equipment.MyRotator;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;
using System;

namespace AlexHelms.NINA.Prometheusexporter.NinaMetrics;

public class RotatorMetrics : IRotatorConsumer
{
    private readonly IRotatorMediator _rotator;
    private readonly PrometheusExporterOptions _options;

    private static readonly string[] Labels = ["rotator_name"];
    private static readonly Gauge MechanicalPosition = Metrics.CreateGauge("nina_rotator_mechanical_position", "Rotator mechanical position in degrees.", Labels);
    private static readonly Gauge SkyPosition = Metrics.CreateGauge("nina_rotator_position", "Rotator position in degrees.", Labels);

    public RotatorMetrics(IRotatorMediator rotator, PrometheusExporterOptions options)
    {
        _options = options;
        _rotator = rotator;
        _rotator.RegisterConsumer(this);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _rotator.RemoveConsumer(this);
    }

    public void UpdateDeviceInfo(RotatorInfo deviceInfo)
    {
        var labels = new[] { deviceInfo.Name ?? "none" };

        if (_options.EnableRotatorMetrics && deviceInfo.Connected)
        {
            MechanicalPosition.WithLabels(labels).Set(Util.ReplaceNan(deviceInfo.MechanicalPosition));
            SkyPosition.WithLabels(labels).Set(Util.ReplaceNan(deviceInfo.Position));
        }
        else
        {
            MechanicalPosition.WithLabels(labels).Unpublish();
            SkyPosition.WithLabels(labels).Unpublish();
        }
    }
}
