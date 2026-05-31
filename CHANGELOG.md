# Changelog

## 0.3.0 (2026-05-30)

- Add `GeoCalculator.DestinationPoint(start, bearingDegrees, distanceKm)` to compute the destination after travelling a distance on an initial bearing
- Add `GeoCalculator.IntermediatePoint(a, b, fraction)` to interpolate along the great-circle path between two coordinates
- Add card image to README

## 0.2.1 (2026-03-31)

- Standardize README to 3-badge format with emoji Support section
- Update CI actions to v5 for Node.js 24 compatibility

## 0.2.0 (2026-03-28)

- Add `GeoCalculator.Midpoint` for computing the geographic center between two coordinates
- Add `GeoCalculator.Bearing` for calculating the initial bearing (forward azimuth) between two coordinates
- Add `GeoCalculator.ClosestTo` for finding the K nearest coordinates to a target point
- Add `GeoCoordinate.Parse` and `GeoCoordinate.TryParse` supporting both decimal and DMS notation
- Add tests for all new functionality
- Fix README compliance: add missing badges, Support section, and License format

## 0.1.2 (2026-03-24)

- Add unit tests
- Add test step to CI workflow

## 0.1.1 (2026-03-23)

- Convert API documentation from bullet lists to table format
- Fix license section formatting

## 0.1.0 (2026-03-22)

- Initial release
- Haversine distance calculation between coordinates
- Support for Kilometers, Miles, Meters, and NauticalMiles
- Bounding box computation for radius searches
- Point-within-radius filtering
