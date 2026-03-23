namespace Philiprehberger.Geolocation;

/// <summary>
/// Represents a geographic coordinate with latitude and longitude.
/// </summary>
/// <remarks>
/// Latitude must be between -90 and 90 degrees.
/// Longitude must be between -180 and 180 degrees.
/// </remarks>
public readonly record struct GeoCoordinate
{
    /// <summary>
    /// Gets the latitude in degrees (-90 to 90).
    /// </summary>
    public double Latitude { get; }

    /// <summary>
    /// Gets the longitude in degrees (-180 to 180).
    /// </summary>
    public double Longitude { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoCoordinate"/> struct.
    /// </summary>
    /// <param name="latitude">The latitude in degrees (-90 to 90).</param>
    /// <param name="longitude">The longitude in degrees (-180 to 180).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when latitude is not between -90 and 90, or longitude is not between -180 and 180.
    /// </exception>
    public GeoCoordinate(double latitude, double longitude)
    {
        if (latitude is < -90.0 or > 90.0)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), latitude, "Latitude must be between -90 and 90 degrees.");
        }

        if (longitude is < -180.0 or > 180.0)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude), longitude, "Longitude must be between -180 and 180 degrees.");
        }

        Latitude = latitude;
        Longitude = longitude;
    }
}
