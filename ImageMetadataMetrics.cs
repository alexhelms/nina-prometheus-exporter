using NINA.WPF.Base.Interfaces.Mediator;
using Prometheus;
using System;

namespace AlexHelms.NINA.PrometheusExporter
{
    public class ImageMetadataMetrics : IDisposable
    {
        private readonly IImageSaveMediator _images;
        private readonly PrometheusExporterOptions _options;

        private static readonly string[] Labels = new[] { "target_name", "filter" };
        private static readonly Gauge ImageScale = Metrics.CreateGauge("nina_image_scale", "Image scale in arcsec/px.", Labels);
        private static readonly Gauge ImageExposureTime = Metrics.CreateGauge("nina_image_exposure_time", "Image exposure time in seconds.", Labels);
        private static readonly Gauge ImageMean = Metrics.CreateGauge("nina_image_mean", "Image mean.", Labels);
        private static readonly Gauge ImageMedian = Metrics.CreateGauge("nina_image_median", "Image median.", Labels);
        private static readonly Gauge ImageMAD = Metrics.CreateGauge("nina_image_mad", "Image median absolute deviation.", Labels);
        private static readonly Gauge ImageStdDev = Metrics.CreateGauge("nina_image_stddev", "Image standard deviation.", Labels);
        private static readonly Gauge ImageMin = Metrics.CreateGauge("nina_image_min", "Image minimum ADU.", Labels);
        private static readonly Gauge ImageMinCount = Metrics.CreateGauge("nina_image_min_count", "Image minimum ADU count.", Labels);
        private static readonly Gauge ImageMax = Metrics.CreateGauge("nina_image_max", "Image maximum ADU.", Labels);
        private static readonly Gauge ImageMaxCount = Metrics.CreateGauge("nina_image_max_count", "Image maximum ADU count.", Labels);
        private static readonly Gauge ImageHfr = Metrics.CreateGauge("nina_image_hfr", "Image HFR in pixels.", Labels);
        private static readonly Gauge ImageHfrStdDev = Metrics.CreateGauge("nina_image_hfr_stddev", "Image HFR standard deviation in pixels.", Labels);
        private static readonly Gauge ImageStarCount = Metrics.CreateGauge("nina_image_star_count", "Image star count.", Labels);

        public ImageMetadataMetrics(IImageSaveMediator images, PrometheusExporterOptions options)
        {
            _images = images;
            _options = options;
            _images.ImageSaved += OnImageSaved;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _images.ImageSaved -= OnImageSaved;
        }

        private void OnImageSaved(object sender, ImageSavedEventArgs e)
        {
            var labels = new[]
            {
                e.MetaData.Target.Name ?? string.Empty,
                e.Filter ?? string.Empty,
            };

            if (_options.EnableImageMetadataMetrics)
            {
                var pixelSize = e.MetaData.Camera.PixelSize;
                var focalLength = e.MetaData.Telescope.FocalLength;
                var binning = int.Parse(e.MetaData.Image.Binning.Substring(0, 1));  // Binning is like "1x1" or "2x2", asymmetric not supported.
                var imageScale = pixelSize / focalLength * 57.29577951308 * 3600.0 / 1000.0 * binning;

                ImageScale.WithLabels(labels).Set(imageScale);
                ImageExposureTime.WithLabels(labels).Set(e.Duration);

                if (e.Statistics != null)
                {
                    ImageMean.WithLabels(labels).Set(e.Statistics.Mean);
                    ImageMedian.WithLabels(labels).Set(e.Statistics.Median);
                    ImageMAD.WithLabels(labels).Set(e.Statistics.MedianAbsoluteDeviation);
                    ImageStdDev.WithLabels(labels).Set(e.Statistics.StDev);
                    ImageMin.WithLabels(labels).Set(e.Statistics.Min);
                    ImageMinCount.WithLabels(labels).Set(e.Statistics.MinOccurrences);
                    ImageMax.WithLabels(labels).Set(e.Statistics.Max);
                    ImageMaxCount.WithLabels(labels).Set(e.Statistics.MaxOccurrences);
                }
                else
                {
                    ImageMean.WithLabels(labels).Unpublish();
                    ImageMedian.WithLabels(labels).Unpublish();
                    ImageMAD.WithLabels(labels).Unpublish();
                    ImageStdDev.WithLabels(labels).Unpublish();
                    ImageMin.WithLabels(labels).Unpublish();
                    ImageMinCount.WithLabels(labels).Unpublish();
                    ImageMax.WithLabels(labels).Unpublish();
                    ImageMaxCount.WithLabels(labels).Unpublish();
                }

                if (e.StarDetectionAnalysis != null)
                {
                    ImageHfr.WithLabels(labels).Set(e.StarDetectionAnalysis.HFR);
                    ImageHfrStdDev.WithLabels(labels).Set(e.StarDetectionAnalysis.HFRStDev);
                    ImageStarCount.WithLabels(labels).Set(e.StarDetectionAnalysis.DetectedStars);
                }
                else
                {
                    ImageHfr.WithLabels(labels).Unpublish();
                    ImageHfrStdDev.WithLabels(labels).Unpublish();
                    ImageStarCount.WithLabels(labels).Unpublish();
                }
            }
            else
            {
                ImageExposureTime.WithLabels(labels).Unpublish();
                ImageMean.WithLabels(labels).Unpublish();
                ImageMedian.WithLabels(labels).Unpublish();
                ImageMAD.WithLabels(labels).Unpublish();
                ImageStdDev.WithLabels(labels).Unpublish();
                ImageMin.WithLabels(labels).Unpublish();
                ImageMinCount.WithLabels(labels).Unpublish();
                ImageMax.WithLabels(labels).Unpublish();
                ImageMaxCount.WithLabels(labels).Unpublish();
                ImageHfr.WithLabels(labels).Unpublish();
                ImageHfrStdDev.WithLabels(labels).Unpublish();
                ImageStarCount.WithLabels(labels).Unpublish();
            }
        }
    }
}
