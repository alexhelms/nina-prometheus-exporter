using NINA.Equipment.Equipment.MyRotator;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;

namespace AlexHelms.NINA.PrometheusExporter
{
    public class RotatorMetrics : IRotatorConsumer
    {
        private readonly IRotatorMediator _rotator;
        private readonly PrometheusExporterOptions _options;

        private static readonly string[] Labels = new[] { "rotator_name" };
        private static readonly Gauge IsConnected = Metrics.CreateGauge("nina_rotator_connected", "NINA rotator connected indicator.", Labels);
        private static readonly Gauge MechanicalPosition = Metrics.CreateGauge("nina_rotator_mechanical_position", "NINA rotator mechanical position in degrees.", Labels);
        private static readonly Gauge SkyPosition = Metrics.CreateGauge("nina_rotator_sky_position", "NINA rotator sky position in degrees.", Labels);

        public RotatorMetrics(IRotatorMediator rotator, PrometheusExporterOptions options)
        {
            _options = options;
            _rotator = rotator;
            _rotator.RegisterConsumer(this);
        }

        public void Dispose()
        {
            _rotator.RemoveConsumer(this);
        }

        public void UpdateDeviceInfo(RotatorInfo deviceInfo)
        {
            if (!deviceInfo.Connected)
                return;

            var labels = new[] { deviceInfo.Name ?? "none" };

            if (_options.EnableRotatorMetrics)
            {
                IsConnected.WithLabels(labels).Set(deviceInfo.Connected ? 1 : 0);
                MechanicalPosition.WithLabels(labels).Set(deviceInfo.MechanicalPosition);
                SkyPosition.WithLabels(labels).Set(deviceInfo.Position);
            }
            else
            {
                IsConnected.WithLabels(labels).Unpublish();
                MechanicalPosition.WithLabels(labels).Unpublish();
                SkyPosition.WithLabels(labels).Unpublish();
            }
        }
    }
}
