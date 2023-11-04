using NINA.Equipment.Equipment.MyFocuser;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;

namespace AlexHelms.NINA.PrometheusExporter
{
    public class FocuserMetrics : IFocuserConsumer
    {
        private readonly IFocuserMediator _focuser;
        private readonly PrometheusExporterOptions _options;

        private static readonly string[] Labels = new[] { "focuser_name" };
        private static readonly Gauge IsConnected = Metrics.CreateGauge("nina_focuser_connected", "NINA focuser connected indicator.", Labels);
        private static readonly Gauge Temperature = Metrics.CreateGauge("nina_focuser_temperature", "NINA focuser temperature in degrees C.", Labels);
        private static readonly Gauge Position = Metrics.CreateGauge("nina_focuser_position", "NINA focuser position in steps.", Labels);
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
            _focuser.RemoveConsumer(this);
        }

        public void UpdateDeviceInfo(FocuserInfo deviceInfo)
        {
            _name = deviceInfo.Name ?? "none";
            var labels = new[] { _name };

            if (_options.EnableFocuserMetrics)
            {
                IsConnected.WithLabels(labels).Set(deviceInfo.Connected ? 1 : 0);
                Temperature.WithLabels(labels).Set(deviceInfo.Temperature);
                Position.WithLabels(labels).Set(deviceInfo.Position);
            }
            else
            {
                IsConnected.WithLabels(labels).Unpublish();
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
}
