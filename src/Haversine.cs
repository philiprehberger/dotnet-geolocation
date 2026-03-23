namespace Philiprehberger.Geolocation;

/// <summary>
/// Implements the Haversine formula for calculating great-circle distances
/// between two points on a sphere.
/// </summary>
public static class Haversine
{
    /// <summary>
    /// Mean radius of the Earth in kilometers.
    /// </summary>
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Calculates the great-circle distance between two geographic coordinates
    /// using the Haversine formula.
    /// </summary>
    /// <param name="coord1">The first coordinate.</param>
    /// <param name="coord2">The second coordinate.</param>
    /// <returns>The distance in kilometers.</returns>
    /// <remarks>
    /// Formula: a = sin²(dlat/2) + cos(lat1) * cos(lat2) * sin²(dlon/2)
    ///          d = 2R * atan2(sqrt(a), sqrt(1-a))
    /// where R = 6371 km (mean Earth radius).
    /// </remarks>
    public static double Calculate(GeoCoordinate coord1, GeoCoordinate coord2)
    {
        var lat1 = DegreesToRadians(coord1.Latitude);
        var lat2 = DegreesToRadians(coord2.Latitude);
        var deltaLat = DegreesToRadians(coord2.Latitude - coord1.Latitude);
        var deltaLon = DegreesToRadians(coord2.Longitude - coord1.Longitude);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2)
              + Math.Cos(lat1) * Math.Cos(lat2)
              * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
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
}
