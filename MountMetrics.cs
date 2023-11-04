using NINA.Equipment.Equipment.MyTelescope;
using NINA.Equipment.Interfaces.Mediator;
using Prometheus;

namespace AlexHelms.NINA.PrometheusExporter
{
    public class MountMetrics : ITelescopeConsumer
    {
        private readonly ITelescopeMediator _mount;
        private readonly PrometheusExporterOptions _options;

        private static readonly string[] Labels = new[] { "mount_name" };
        private static readonly Gauge RightAscension = Metrics.CreateGauge("nina_mount_ra", "Right Ascension of the mount in degrees.", Labels);
        private static readonly Gauge Declination = Metrics.CreateGauge("nina_mount_dec", "Declination of the mount in degrees.", Labels);
        private static readonly Gauge Altitude = Metrics.CreateGauge("nina_mount_alt", "Altitude of the mount in degrees.", Labels);
        private static readonly Gauge Azimuth = Metrics.CreateGauge("nina_mount_az", "Azimuth of the mount in degrees.", Labels);
        private static readonly Gauge SideOfPierEast = Metrics.CreateGauge("nina_mount_side_of_pier_east", "Mount side of pier indicator, 1 when telescope is on east side of pier.", Labels);
        private static readonly Gauge SideOfPierWest = Metrics.CreateGauge("nina_mount_side_of_pier_west", "Mount side of pier indicator, 1 when telescope is on west side of pier.", Labels);


        public MountMetrics(ITelescopeMediator mount, PrometheusExporterOptions options)
        {
            _mount = mount;
            _options = options;
            _mount.RegisterConsumer(this);
        }

        public void Dispose()
        {
            _mount.RemoveConsumer(this);
        }

        public void UpdateDeviceInfo(TelescopeInfo deviceInfo)
        {
            var labels = new[] { deviceInfo.Name ?? "none" };

            if (_options.EnableMountMetrics)
            {
                double sideOfPierEast = double.NaN;
                double sideOfPierWest = double.NaN;

                if (deviceInfo.SideOfPier == global::NINA.Core.Enum.PierSide.pierEast)
                {
                    sideOfPierEast = 1;
                    sideOfPierWest = 0;
                }
                else if (deviceInfo.SideOfPier == global::NINA.Core.Enum.PierSide.pierWest)
                {
                    sideOfPierEast = 0;
                    sideOfPierWest = 1;
                }

                RightAscension.WithLabels(labels).Set(deviceInfo.Coordinates?.RADegrees ?? double.NaN);
                Declination.WithLabels(labels).Set(deviceInfo.Coordinates?.Dec ?? double.NaN);
                Altitude.WithLabels(labels).Set(deviceInfo.Altitude);
                Azimuth.WithLabels(labels).Set(deviceInfo.Azimuth);
                SideOfPierEast.WithLabels(labels).Set(sideOfPierEast);
                SideOfPierWest.WithLabels(labels).Set(sideOfPierWest);
            }
            else
            {
                RightAscension.WithLabels(labels).Unpublish();
                Declination.WithLabels(labels).Unpublish();
                Altitude.WithLabels(labels).Unpublish();
                Azimuth.WithLabels(labels).Unpublish();
                SideOfPierEast.WithLabels(labels).Unpublish();
                SideOfPierWest.WithLabels(labels).Unpublish();
            }
        }
    }
}
