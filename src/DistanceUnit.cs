namespace Philiprehberger.Geolocation;

/// <summary>
/// Represents distance measurement units with conversion factors from kilometers.
/// </summary>
public enum DistanceUnit
{
    /// <summary>Kilometers (base unit).</summary>
    Kilometers = 0,

    /// <summary>Miles (1 km = 0.621371 mi).</summary>
    Miles,

    /// <summary>Meters (1 km = 1000 m).</summary>
    Meters,

    /// <summary>Nautical miles (1 km = 0.539957 nmi).</summary>
    NauticalMiles
}

/// <summary>
/// Extension methods for <see cref="DistanceUnit"/> conversion.
/// </summary>
internal static class DistanceUnitExtensions
{
    /// <summary>
    /// Gets the conversion factor from kilometers for the specified unit.
    /// </summary>
    /// <param name="unit">The distance unit.</param>
    /// <returns>The conversion factor to multiply kilometers by.</returns>
    internal static double ConversionFactor(this DistanceUnit unit)
    {
        return unit switch
        {
            DistanceUnit.Kilometers => 1.0,
            DistanceUnit.Miles => 0.621371,
            DistanceUnit.Meters => 1000.0,
            DistanceUnit.NauticalMiles => 0.539957,
            _ => 1.0
        };
    }
}
