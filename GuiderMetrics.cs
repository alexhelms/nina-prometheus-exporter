using NINA.Equipment.Equipment.MyGuider;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;
using System;

namespace AlexHelms.NINA.PrometheusExporter
{
    public class GuiderMetrics : IGuiderConsumer
    {
        private readonly IGuiderMediator _guider;
        private readonly PrometheusExporterOptions _options;

        private static readonly Gauge RaError = Metrics.CreateGauge("nina_guider_ra", "Guider RA error in arcsec.");
        private static readonly Gauge DecError = Metrics.CreateGauge("nina_guider_dec", "Guider DEC error in arcsec.");
        private static readonly Gauge TotalError = Metrics.CreateGauge("nina_guider_total", "Guider total error in arcsec.");
        private static readonly Histogram TotalErrorHisto = Metrics.CreateHistogram("nina_guider_total_histo", "Histogram of guider total error in arcsec.", new HistogramConfiguration
        {
            Buckets = Histogram.ExponentialBuckets(0.1, 1.1, 50),
        });

        private double _pixelScale = 1;

        public GuiderMetrics(IGuiderMediator guider, PrometheusExporterOptions options)
        {
            _guider = guider;
            _options = options;
            _guider.RegisterConsumer(this);
            _guider.GuideEvent += OnGuideEvent;
        }

        public void Dispose()
        {
            _guider.GuideEvent -= OnGuideEvent;
            _guider.RemoveConsumer(this);
        }

        private void OnGuideEvent(object sender, global::NINA.Core.Interfaces.IGuideStep e)
        {
            if (_options.EnableGuiderMetrics)
            {
                var ra = e.RADistanceRaw * _pixelScale;
                var dec = e.DECDistanceRaw * _pixelScale;
                var total = Math.Sqrt(ra * ra + dec * dec);

                RaError.Set(ra);
                DecError.Set(dec);
                TotalError.Set(total);
                TotalErrorHisto.Observe(total);
            }
            else
            {
                RaError.Unpublish();
                DecError.Unpublish();
                TotalError.Unpublish();
                TotalErrorHisto.Unpublish();
            }
        }

        public void UpdateDeviceInfo(GuiderInfo deviceInfo)
        {
            _pixelScale = deviceInfo.PixelScale;
        }
    }
}
