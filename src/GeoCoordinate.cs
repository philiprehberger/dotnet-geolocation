using System.Globalization;
using System.Text.RegularExpressions;

namespace Philiprehberger.Geolocation;

/// <summary>
/// Represents a geographic coordinate with latitude and longitude.
/// </summary>
/// <remarks>
/// Latitude must be between -90 and 90 degrees.
/// Longitude must be between -180 and 180 degrees.
/// </remarks>
public readonly partial record struct GeoCoordinate
{
    /// <summary>
    /// Regular expression pattern for parsing DMS (degrees, minutes, seconds) notation.
    /// Matches formats like <c>40°26'46"N 79°58'56"W</c> or <c>40°26'46.5"N 79°58'56.3"W</c>.
    /// </summary>
    private const string DmsPattern =
        @"^\s*(\d+)[°]\s*(\d+)[''′]\s*([\d.]+)[""″]\s*([NnSs])\s+(\d+)[°]\s*(\d+)[''′]\s*([\d.]+)[""″]\s*([EeWw])\s*$";

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

    /// <summary>
    /// Parses a string representation of a coordinate in either decimal (<c>48.2082, 16.3738</c>)
    /// or DMS (<c>40°26'46"N 79°58'56"W</c>) notation.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <returns>A <see cref="GeoCoordinate"/> parsed from the input string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
    /// <exception cref="FormatException">Thrown when the input is not a valid coordinate format.</exception>
    public static GeoCoordinate Parse(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (TryParse(input, out var result))
        {
            return result;
        }

        throw new FormatException($"'{input}' is not a valid coordinate format. Expected decimal (e.g. '48.2082, 16.3738') or DMS (e.g. '40°26'46\"N 79°58'56\"W').");
    }

    /// <summary>
    /// Attempts to parse a string representation of a coordinate in either decimal (<c>48.2082, 16.3738</c>)
    /// or DMS (<c>40°26'46"N 79°58'56"W</c>) notation.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed coordinate if successful; otherwise the default value.</param>
    /// <returns><c>true</c> if the string was parsed successfully; otherwise <c>false</c>.</returns>
    public static bool TryParse(string? input, out GeoCoordinate result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        // Try DMS format first
        if (TryParseDms(input, out result))
        {
            return true;
        }

        // Try decimal format: "lat, lon"
        if (TryParseDecimal(input, out result))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to parse a DMS (degrees, minutes, seconds) string.
    /// </summary>
    private static bool TryParseDms(string input, out GeoCoordinate result)
    {
        result = default;

        var match = DmsRegex().Match(input);
        if (!match.Success)
        {
            return false;
        }

        if (!double.TryParse(match.Groups[1].Value, CultureInfo.InvariantCulture, out var latDeg) ||
            !double.TryParse(match.Groups[2].Value, CultureInfo.InvariantCulture, out var latMin) ||
            !double.TryParse(match.Groups[3].Value, CultureInfo.InvariantCulture, out var latSec) ||
            !double.TryParse(match.Groups[5].Value, CultureInfo.InvariantCulture, out var lonDeg) ||
            !double.TryParse(match.Groups[6].Value, CultureInfo.InvariantCulture, out var lonMin) ||
            !double.TryParse(match.Groups[7].Value, CultureInfo.InvariantCulture, out var lonSec))
        {
            return false;
        }

        var latDirection = match.Groups[4].Value.ToUpperInvariant();
        var lonDirection = match.Groups[8].Value.ToUpperInvariant();

        var latitude = latDeg + latMin / 60.0 + latSec / 3600.0;
        var longitude = lonDeg + lonMin / 60.0 + lonSec / 3600.0;

        if (latDirection == "S")
        {
            latitude = -latitude;
        }

        if (lonDirection == "W")
        {
            longitude = -longitude;
        }

        if (latitude is < -90.0 or > 90.0 || longitude is < -180.0 or > 180.0)
        {
            return false;
        }

        result = new GeoCoordinate(latitude, longitude);
        return true;
    }

    /// <summary>
    /// Attempts to parse a decimal coordinate string in the format "latitude, longitude".
    /// </summary>
    private static bool TryParseDecimal(string input, out GeoCoordinate result)
    {
        result = default;

        var parts = input.Split(',');
        if (parts.Length != 2)
        {
            return false;
        }

        if (!double.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude) ||
            !double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
        {
            return false;
        }

        if (latitude is < -90.0 or > 90.0 || longitude is < -180.0 or > 180.0)
        {
            return false;
        }

        result = new GeoCoordinate(latitude, longitude);
        return true;
    }

    [GeneratedRegex(DmsPattern)]
    private static partial Regex DmsRegex();
}
