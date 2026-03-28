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
