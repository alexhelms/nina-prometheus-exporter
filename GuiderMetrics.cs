using NINA.Equipment.Equipment.MyGuider;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;

namespace AlexHelms.NINA.PrometheusExporter
{
    public class GuiderMetrics : IGuiderConsumer
    {
        private readonly IGuiderMediator _guider;
        private readonly PrometheusExporterOptions _options;

        private static readonly Gauge RaRMS = Metrics.CreateGauge("nina_guider_ra_rms", "Guider RA RMS error in arcsec.");
        private static readonly Gauge DecRMS = Metrics.CreateGauge("nina_guider_dec_rms", "Guider DEC RMS error in arcsec.");
        private static readonly Gauge TotalRMS = Metrics.CreateGauge("nina_guider_total_rms", "Guider total RMS error in arcsec.");

        public GuiderMetrics(IGuiderMediator guider, PrometheusExporterOptions options)
        {
            _guider = guider;
            _options = options;
            _guider.RegisterConsumer(this);
        }

        public void Dispose()
        {
            _guider.RemoveConsumer(this);
        }

        public void UpdateDeviceInfo(GuiderInfo deviceInfo)
        {
            if (_options.EnableGuiderMetrics)
            {
                RaRMS.Set(deviceInfo.RMSError?.RA.Arcseconds ?? double.NaN);
                DecRMS.Set(deviceInfo.RMSError?.Dec.Arcseconds ?? double.NaN);
                TotalRMS.Set(deviceInfo.RMSError?.Total.Arcseconds ?? double.NaN);
            }
            else
            {
                RaRMS.Unpublish();
                DecRMS.Unpublish();
                TotalRMS.Unpublish();
            }
        }
    }
}
