using NINA.Equipment.Equipment.MySafetyMonitor;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;

namespace AlexHelms.NINA.PrometheusExporter
{
    public class SafetyMetrics : ISafetyMonitorConsumer
    {
        private readonly ISafetyMonitorMediator _safety;
        private readonly PrometheusExporterOptions _options;

        private static readonly string[] Labels = new[] { "safety_name" };
        private readonly Gauge Safe = Metrics.CreateGauge("nina_safety_safe", "Safety monitor indicator, 1 is safe.", Labels);

        public SafetyMetrics(ISafetyMonitorMediator safety, PrometheusExporterOptions options)
        {
            _safety = safety;
            _options = options;
            _safety.RegisterConsumer(this);
        }

        public void Dispose()
        {
            _safety.RemoveConsumer(this);
        }

        public void UpdateDeviceInfo(SafetyMonitorInfo deviceInfo)
        {
            if (!deviceInfo.Connected)
                return;

            var labels = new[] { deviceInfo.Name ?? "none" };

            if (_options.EnableSafetyMetrics)
            {
                Safe.WithLabels(labels).Set(deviceInfo.IsSafe ? 1 : 0);
            }
            else
            {
                Safe.WithLabels(labels).Unpublish();
            }
        }
    }
}
