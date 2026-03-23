namespace Philiprehberger.Geolocation;

/// <summary>
/// Provides static methods for geographic calculations including distance,
/// bounding boxes, and radius filtering.
/// </summary>
public static class Geo
{
    /// <summary>
    /// Mean radius of the Earth in kilometers.
    /// </summary>
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Calculates the great-circle distance between two coordinates using the Haversine formula.
    /// </summary>
    /// <param name="coord1">The first coordinate.</param>
    /// <param name="coord2">The second coordinate.</param>
    /// <param name="unit">The unit of measurement for the result. Defaults to <see cref="DistanceUnit.Kilometers"/>.</param>
    /// <returns>The distance in the specified unit.</returns>
    public static double Distance(GeoCoordinate coord1, GeoCoordinate coord2, DistanceUnit unit = DistanceUnit.Kilometers)
    {
        var km = Haversine.Calculate(coord1, coord2);
        return km * unit.ConversionFactor();
    }

    /// <summary>
    /// Computes a bounding box (minimum and maximum coordinates) around a center point
    /// for the given radius in kilometers.
    /// </summary>
    /// <param name="center">The center coordinate.</param>
    /// <param name="radiusKm">The radius in kilometers.</param>
    /// <returns>A tuple of (min, max) <see cref="GeoCoordinate"/> defining the bounding box.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="radiusKm"/> is negative.</exception>
    public static (GeoCoordinate Min, GeoCoordinate Max) BoundingBox(GeoCoordinate center, double radiusKm)
    {
        if (radiusKm < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(radiusKm), radiusKm, "Radius must be non-negative.");
        }

        // Angular distance in radians on Earth's surface
        var angularDistance = radiusKm / EarthRadiusKm;

        var latRad = center.Latitude * Math.PI / 180.0;
        var lonRad = center.Longitude * Math.PI / 180.0;

        var minLat = (latRad - angularDistance) * 180.0 / Math.PI;
        var maxLat = (latRad + angularDistance) * 180.0 / Math.PI;

        var deltaLon = Math.Asin(Math.Sin(angularDistance) / Math.Cos(latRad));
        var minLon = (lonRad - deltaLon) * 180.0 / Math.PI;
        var maxLon = (lonRad + deltaLon) * 180.0 / Math.PI;

        // Clamp latitude
        minLat = Math.Max(minLat, -90.0);
        maxLat = Math.Min(maxLat, 90.0);

        // Clamp longitude
        minLon = Math.Max(minLon, -180.0);
        maxLon = Math.Min(maxLon, 180.0);

        return (new GeoCoordinate(minLat, minLon), new GeoCoordinate(maxLat, maxLon));
    }

    /// <summary>
    /// Checks whether a point is within the specified radius (in kilometers) of a center coordinate.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <param name="center">The center coordinate.</param>
    /// <param name="radiusKm">The radius in kilometers.</param>
    /// <returns><c>true</c> if the point is within the radius; otherwise <c>false</c>.</returns>
    public static bool IsWithinRadius(GeoCoordinate point, GeoCoordinate center, double radiusKm)
    {
        var distance = Haversine.Calculate(center, point);
        return distance <= radiusKm;
    }

    /// <summary>
    /// Filters a collection of coordinates, returning only those within the specified radius
    /// of the center point.
    /// </summary>
    /// <param name="points">The coordinates to filter.</param>
    /// <param name="center">The center coordinate.</param>
    /// <param name="radiusKm">The radius in kilometers.</param>
    /// <returns>An enumerable of coordinates within the radius.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is null.</exception>
    public static IEnumerable<GeoCoordinate> Filter(IEnumerable<GeoCoordinate> points, GeoCoordinate center, double radiusKm)
    {
        ArgumentNullException.ThrowIfNull(points);

        return points.Where(point => IsWithinRadius(point, center, radiusKm));
    }
}
