using AlexHelms.NINA.PrometheusExporter;
using NINA.Core.Interfaces;
using NINA.Equipment.Equipment.MyGuider;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;
using System;

namespace AlexHelms.NINA.Prometheusexporter.NinaMetrics;

public class GuiderMetrics : IGuiderConsumer
{
    private readonly IGuiderMediator _guider;
    private readonly PrometheusExporterOptions _options;

    private static readonly Gauge RaErrorArcsec = Metrics.CreateGauge("nina_guider_ra_arcsec", "Guider RA error in arcsec.");
    private static readonly Gauge DecErrorArcsec = Metrics.CreateGauge("nina_guider_dec_arcsec", "Guider DEC error in arcsec.");
    private static readonly Gauge TotalErrorArcsec = Metrics.CreateGauge("nina_guider_total_arcsec", "Guider total error in arcsec.");

    private static readonly Gauge RaErrorPixel = Metrics.CreateGauge("nina_guider_ra_pixel", "Guider RA error in pixels.");
    private static readonly Gauge DecErrorPixel = Metrics.CreateGauge("nina_guider_dec_pixel", "Guider DEC error in pixels.");
    private static readonly Gauge TotalErrorPixel = Metrics.CreateGauge("nina_guider_total_pixel", "Guider total error in pixels.");

    private static readonly Histogram TotalErrorHistoArcsec = Metrics.CreateHistogram("nina_guider_total_arcsec_histo", "Histogram of guider total error in arcsec.", new HistogramConfiguration
    {
        Buckets = Histogram.ExponentialBuckets(0.1, 1.1, 50),
    });

    private static readonly Histogram TotalErrorHistoPixel = Metrics.CreateHistogram("nina_guider_total_arcsec_histo", "Histogram of guider total error in pixels.", new HistogramConfiguration
    {
        Buckets = Histogram.ExponentialBuckets(0.01, 1.1, 50),
    });

    private double _pixelScale = 1;
    private GuiderInfo _guiderInfo = new();

    public GuiderMetrics(IGuiderMediator guider, PrometheusExporterOptions options)
    {
        _guider = guider;
        _options = options;
        _guider.RegisterConsumer(this);
        _guider.GuideEvent += OnGuideEvent;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _guider.GuideEvent -= OnGuideEvent;
        _guider.RemoveConsumer(this);
    }

    private void OnGuideEvent(object sender, IGuideStep e)
    {
        if (_options.EnableGuiderMetrics && _guiderInfo.Connected)
        {
            RaErrorArcsec.Set(Util.ReplaceNan(_guiderInfo.RMSError.RA.Arcseconds));
            DecErrorArcsec.Set(Util.ReplaceNan(_guiderInfo.RMSError.Dec.Arcseconds));
            TotalErrorArcsec.Set(Util.ReplaceNan(_guiderInfo.RMSError.Total.Arcseconds));
            RaErrorPixel.Set(Util.ReplaceNan(_guiderInfo.RMSError.RA.Pixel));
            DecErrorPixel.Set(Util.ReplaceNan(_guiderInfo.RMSError.Dec.Pixel));
            TotalErrorPixel.Set(Util.ReplaceNan(_guiderInfo.RMSError.Total.Pixel));
            TotalErrorHistoArcsec.Observe(Util.ReplaceNan(_guiderInfo.RMSError.Total.Arcseconds));
            TotalErrorHistoPixel.Observe(Util.ReplaceNan(_guiderInfo.RMSError.Total.Pixel));
        }
        else
        {
            RaErrorArcsec.Unpublish();
            DecErrorArcsec.Unpublish();
            TotalErrorArcsec.Unpublish();
            RaErrorPixel.Unpublish();
            DecErrorPixel.Unpublish();
            TotalErrorPixel.Unpublish();
            TotalErrorHistoArcsec.Unpublish();
            TotalErrorHistoPixel.Unpublish();
        }
    }

    public void UpdateDeviceInfo(GuiderInfo deviceInfo)
    {
        _guiderInfo = deviceInfo;
    }
}
