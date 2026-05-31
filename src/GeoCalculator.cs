namespace Philiprehberger.Geolocation;

/// <summary>
/// Provides advanced geographic calculations including midpoint, bearing,
/// and nearest-neighbor queries.
/// </summary>
public static class GeoCalculator
{
    /// <summary>
    /// Calculates the geographic midpoint between two coordinates using the spherical midpoint formula.
    /// </summary>
    /// <param name="a">The first coordinate.</param>
    /// <param name="b">The second coordinate.</param>
    /// <returns>A <see cref="GeoCoordinate"/> representing the geographic center between the two points.</returns>
    /// <remarks>
    /// Uses the spherical midpoint formula:
    /// Bx = cos(lat2) * cos(dLon)
    /// By = cos(lat2) * sin(dLon)
    /// midLat = atan2(sin(lat1) + sin(lat2), sqrt((cos(lat1) + Bx)^2 + By^2))
    /// midLon = lon1 + atan2(By, cos(lat1) + Bx)
    /// </remarks>
    public static GeoCoordinate Midpoint(GeoCoordinate a, GeoCoordinate b)
    {
        var lat1 = DegreesToRadians(a.Latitude);
        var lon1 = DegreesToRadians(a.Longitude);
        var lat2 = DegreesToRadians(b.Latitude);
        var lon2 = DegreesToRadians(b.Longitude);

        var dLon = lon2 - lon1;

        var bx = Math.Cos(lat2) * Math.Cos(dLon);
        var by = Math.Cos(lat2) * Math.Sin(dLon);

        var midLat = Math.Atan2(
            Math.Sin(lat1) + Math.Sin(lat2),
            Math.Sqrt((Math.Cos(lat1) + bx) * (Math.Cos(lat1) + bx) + by * by));

        var midLon = lon1 + Math.Atan2(by, Math.Cos(lat1) + bx);

        // Normalize longitude to -180..180
        midLon = (midLon + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

        return new GeoCoordinate(RadiansToDegrees(midLat), RadiansToDegrees(midLon));
    }

    /// <summary>
    /// Calculates the initial bearing (forward azimuth) from one coordinate to another.
    /// </summary>
    /// <param name="from">The starting coordinate.</param>
    /// <param name="to">The destination coordinate.</param>
    /// <returns>The bearing in degrees (0-360), where 0 is north, 90 is east, 180 is south, and 270 is west.</returns>
    /// <remarks>
    /// Uses the forward azimuth formula:
    /// theta = atan2(sin(dLon) * cos(lat2), cos(lat1) * sin(lat2) - sin(lat1) * cos(lat2) * cos(dLon))
    /// bearing = (theta * 180 / PI + 360) % 360
    /// </remarks>
    public static double Bearing(GeoCoordinate from, GeoCoordinate to)
    {
        var lat1 = DegreesToRadians(from.Latitude);
        var lat2 = DegreesToRadians(to.Latitude);
        var dLon = DegreesToRadians(to.Longitude - from.Longitude);

        var y = Math.Sin(dLon) * Math.Cos(lat2);
        var x = Math.Cos(lat1) * Math.Sin(lat2)
              - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

        var theta = Math.Atan2(y, x);
        return (RadiansToDegrees(theta) + 360.0) % 360.0;
    }

    /// <summary>
    /// Finds the closest coordinates to a target point from a collection of candidates.
    /// </summary>
    /// <param name="target">The reference coordinate to measure distances from.</param>
    /// <param name="candidates">The collection of coordinates to search.</param>
    /// <param name="count">The number of closest points to return. Defaults to 1.</param>
    /// <returns>An enumerable of up to <paramref name="count"/> coordinates, ordered by distance from the target.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="candidates"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is less than 1.</exception>
    public static IEnumerable<GeoCoordinate> ClosestTo(GeoCoordinate target, IEnumerable<GeoCoordinate> candidates, int count = 1)
    {
        ArgumentNullException.ThrowIfNull(candidates);

        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be at least 1.");
        }

        return candidates
            .OrderBy(c => Haversine.Calculate(target, c))
            .Take(count);
    }

    /// <summary>
    /// Computes the destination coordinate when travelling a given distance along an initial bearing
    /// from a starting coordinate, using the spherical destination formula.
    /// </summary>
    /// <param name="start">The starting coordinate.</param>
    /// <param name="bearingDegrees">The initial bearing in degrees (0-360, where 0 is north).</param>
    /// <param name="distanceKm">The distance to travel in kilometers.</param>
    /// <returns>The destination <see cref="GeoCoordinate"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="distanceKm"/> is negative.</exception>
    public static GeoCoordinate DestinationPoint(GeoCoordinate start, double bearingDegrees, double distanceKm)
    {
        if (distanceKm < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(distanceKm), distanceKm, "Distance must be non-negative.");
        }

        const double earthRadiusKm = 6371.0;
        var angularDistance = distanceKm / earthRadiusKm;
        var bearingRad = DegreesToRadians(bearingDegrees);
        var lat1 = DegreesToRadians(start.Latitude);
        var lon1 = DegreesToRadians(start.Longitude);

        var lat2 = Math.Asin(
            Math.Sin(lat1) * Math.Cos(angularDistance) +
            Math.Cos(lat1) * Math.Sin(angularDistance) * Math.Cos(bearingRad));

        var lon2 = lon1 + Math.Atan2(
            Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(lat1),
            Math.Cos(angularDistance) - Math.Sin(lat1) * Math.Sin(lat2));

        // Normalize longitude to -180..180
        lon2 = (lon2 + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

        return new GeoCoordinate(RadiansToDegrees(lat2), RadiansToDegrees(lon2));
    }

    /// <summary>
    /// Computes an intermediate coordinate along the great-circle path between two coordinates
    /// at the given fraction of the total path (0.0 = <paramref name="a"/>, 1.0 = <paramref name="b"/>).
    /// </summary>
    /// <param name="a">The start coordinate.</param>
    /// <param name="b">The end coordinate.</param>
    /// <param name="fraction">A value in [0, 1] indicating where along the path to interpolate.</param>
    /// <returns>The interpolated <see cref="GeoCoordinate"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="fraction"/> is outside [0, 1].</exception>
    public static GeoCoordinate IntermediatePoint(GeoCoordinate a, GeoCoordinate b, double fraction)
    {
        if (fraction is < 0.0 or > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(fraction), fraction, "Fraction must be between 0 and 1.");
        }

        var lat1 = DegreesToRadians(a.Latitude);
        var lon1 = DegreesToRadians(a.Longitude);
        var lat2 = DegreesToRadians(b.Latitude);
        var lon2 = DegreesToRadians(b.Longitude);

        var dLat = lat2 - lat1;
        var dLon = lon2 - lon1;

        var aHaversine = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                       + Math.Cos(lat1) * Math.Cos(lat2)
                       * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var delta = 2 * Math.Atan2(Math.Sqrt(aHaversine), Math.Sqrt(1 - aHaversine));

        if (delta == 0.0)
        {
            return a;
        }

        var A = Math.Sin((1 - fraction) * delta) / Math.Sin(delta);
        var B = Math.Sin(fraction * delta) / Math.Sin(delta);

        var x = A * Math.Cos(lat1) * Math.Cos(lon1) + B * Math.Cos(lat2) * Math.Cos(lon2);
        var y = A * Math.Cos(lat1) * Math.Sin(lon1) + B * Math.Cos(lat2) * Math.Sin(lon2);
        var z = A * Math.Sin(lat1) + B * Math.Sin(lat2);

        var midLat = Math.Atan2(z, Math.Sqrt(x * x + y * y));
        var midLon = Math.Atan2(y, x);

        return new GeoCoordinate(RadiansToDegrees(midLat), RadiansToDegrees(midLon));
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">The angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    /// <summary>
    /// Converts radians to degrees.
    /// </summary>
    /// <param name="radians">The angle in radians.</param>
    /// <returns>The angle in degrees.</returns>
    private static double RadiansToDegrees(double radians)
    {
        return radians * 180.0 / Math.PI;
    }
}
