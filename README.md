# Prometheus Exporter Plugin for NINA

The `Prometheus Exporter Plugin for NINA` exposes various image and equipment metrics for `prometheus`
to collect. The exporter creates an HTTP server and [Prometheus](https://prometheus.io/) pulls data
from an exporter.

## Metrics ##

Global Labels:
- `hostname`
- `profile`

### Camera

Labels:
- `camera_name`

| Name | Description |
| ---- | ----------- |
| `nina_camera_sensor_temperature` | Camera sensor temperature in degrees C. |
| `nina_camera_cooler_power` | Camera cooler power, 0 to 100%. |
| `nina_camera_battery_level` | Camera battery level, 0 to 100%. |

### Focuser

Labels:
- `focuser_name`

| Name | Description |
| ---- | ----------- |
| `nina_focuser_temperature` | Focuser temperature in degrees C. |
| `nina_focuser_position` | Focuser position in steps. |
| `nina_autofocus` | Number of autofocus runs. |

### Guider

| Name | Description |
| ---- | ----------- |
| `nina_guider_ra_arcsec` | Guider RA error in arcsec. |
| `nina_guider_dec_arcsec` | Guider DEC error in arcsec. |
| `nina_guider_total_arcsec` | Guider total error in arcsec. |
| `nina_guider_ra_pixel` | Guider RA error in pixels. |
| `nina_guider_dec_pixel` | Guider DEC error in pixels. |
| `nina_guider_total_pixel` | Guider total error in pixels. |
| `nina_guider_total_arcsec_histo` | Histogram of guider total error in arcsec. |
| `nina_guider_total_pixel_histo` | Histogram of guider total error in pixels. |

### Image Metadata

Labels:
- `target_name`
- `filter`

| Name | Description |
| ---- | ----------- |
| `nina_image_scale` | Image scale in arcsec/px. |
| `nina_image_exposure_time` | Image exposure time in seconds. |
| `nina_image_mean` | Image mean. |
| `nina_image_median` | Image median. |
| `nina_image_mad` | Image median absolute deviation. |
| `nina_image_stddev` | Image standard deviation. |
| `nina_image_min` | Image minimum ADU. |
| `nina_image_min_count` | Image minimum ADU count. |
| `nina_image_max` | Image maximum ADU. |
| `nina_image_max_count` | Image maximum ADU count. |
| `nina_image_hfr_pixels` | Image HFR in pixels. |
| `nina_image_hfr_arcsec` | Image HFR in arcsec. |
| `nina_image_hfr_stddev` | Image HFR standard deviation in pixels. |
| `nina_image_star_count` | Image star count. |

### Mount

Labels:
- `mount_name`

| Name | Description |
| ---- | ----------- |
| `nina_mount_ra` | Right Ascension of the mount in degrees. |
| `nina_mount_dec` | Declination of the mount in degrees. |
| `nina_mount_alt` | Altitude of the mount in degrees. |
| `nina_mount_az` | Azimuth of the mount in degrees. |
| `nina_mount_side_of_pier_east` | Mount side of pier indicator, 1 when telescope is on east side of pier. |
| `nina_mount_side_of_pier_west` | Mount side of pier indicator, 1 when telescope is on west side of pier. |

### Other

| Name | Description |
| ---- | ----------- |
| `nina_sun_altitude` | Sun altitude in degrees. |
| `nina_moon_altitude` | Moon altitude in degrees. |
| `nina_moon_illumination` | Moon illuminationm, 0 to 100%. |
| `nina_moon_position_angle` | Moon position angle in degrees. |

### Rotator

Labels:
- `rotator_name`

| Name | Description |
| ---- | ----------- |
| `nina_rotator_mechanical_position` | Rotator mechanical position in degrees. |
| `nina_rotator_position` | Rotator position in degrees. |

### Safety

Labels:
- `safety_name`

| Name | Description |
| ---- | ----------- |
| `nina_safety_safe` | Safety monitor indicator, 1 is safe. |

### Weather

| Name | Description |
| ---- | ----------- |
| `nina_weather_cloud_cover` | Weather cloud cover, 0 to 100%. |
| `nina_weather_dew_point` | Weather dew point in degrees C. |
| `nina_weather_humidity` | Weather humidity, 0 to 100%. |
| `nina_weather_pressure` | Weather pressure in hPa. |
| `nina_weather_rain_rate` | Weather rain rate in mm/hr. |
| `nina_weather_sky_brightness` | Weather sky brightness in lux. |
| `nina_weather_sky_quality` | Weather sky quality in mag/sq-arcsec. |
| `nina_weather_sky_temperature` | Weather sky temperature in degrees C. |
| `nina_weather_star_fwhm` | Weather star FWHM in arcsec. |
| `nina_weather_temperature` | Weather temperature in degrees C. |
| `nina_weather_wind_direction` | Weather wind direction in degrees. |
| `nina_weather_wind_gust` | Weather wind gust in m/s. |
| `nina_weather_wind_speed` | Weather wind speed m/s. |
