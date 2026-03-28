using Xunit;
using Philiprehberger.Geolocation;

namespace Philiprehberger.Geolocation.Tests;

public class GeoCoordinateTests
{
    [Fact]
    public void Constructor_ValidCoordinates_SetsProperties()
    {
        var coord = new GeoCoordinate(48.2082, 16.3738);

        Assert.Equal(48.2082, coord.Latitude);
        Assert.Equal(16.3738, coord.Longitude);
    }

    [Theory]
    [InlineData(-91.0, 0.0)]
    [InlineData(91.0, 0.0)]
    public void Constructor_InvalidLatitude_ThrowsArgumentOutOfRangeException(double latitude, double longitude)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GeoCoordinate(latitude, longitude));
    }

    [Theory]
    [InlineData(0.0, -181.0)]
    [InlineData(0.0, 181.0)]
    public void Constructor_InvalidLongitude_ThrowsArgumentOutOfRangeException(double latitude, double longitude)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GeoCoordinate(latitude, longitude));
    }

    [Fact]
    public void Constructor_BoundaryValues_Succeeds()
    {
        var min = new GeoCoordinate(-90.0, -180.0);
        var max = new GeoCoordinate(90.0, 180.0);

        Assert.Equal(-90.0, min.Latitude);
        Assert.Equal(180.0, max.Longitude);
    }

    // --- Parse (DMS) tests ---

    [Fact]
    public void Parse_DmsFormat_ReturnsCorrectCoordinate()
    {
        var coord = GeoCoordinate.Parse("40°26'46\"N 79°58'56\"W");

        Assert.Equal(40.4461, coord.Latitude, precision: 3);
        Assert.Equal(-79.9822, coord.Longitude, precision: 3);
    }

    [Fact]
    public void Parse_DmsSouthEast_ReturnsNegativeLatitude()
    {
        var coord = GeoCoordinate.Parse("33°51'54\"S 151°12'36\"E");

        Assert.Equal(-33.865, coord.Latitude, precision: 2);
        Assert.Equal(151.21, coord.Longitude, precision: 2);
    }

    [Fact]
    public void Parse_DecimalFormat_ReturnsCorrectCoordinate()
    {
        var coord = GeoCoordinate.Parse("48.2082, 16.3738");

        Assert.Equal(48.2082, coord.Latitude, precision: 4);
        Assert.Equal(16.3738, coord.Longitude, precision: 4);
    }

    [Fact]
    public void Parse_DecimalNegativeValues_ReturnsCorrectCoordinate()
    {
        var coord = GeoCoordinate.Parse("-34.6037, -58.3816");

        Assert.Equal(-34.6037, coord.Latitude, precision: 4);
        Assert.Equal(-58.3816, coord.Longitude, precision: 4);
    }

    [Fact]
    public void Parse_InvalidString_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => GeoCoordinate.Parse("not a coordinate"));
    }

    [Fact]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => GeoCoordinate.Parse(null!));
    }

    // --- TryParse tests ---

    [Fact]
    public void TryParse_ValidDms_ReturnsTrue()
    {
        var success = GeoCoordinate.TryParse("40°26'46\"N 79°58'56\"W", out var result);

        Assert.True(success);
        Assert.Equal(40.4461, result.Latitude, precision: 3);
    }

    [Fact]
    public void TryParse_ValidDecimal_ReturnsTrue()
    {
        var success = GeoCoordinate.TryParse("48.2082, 16.3738", out var result);

        Assert.True(success);
        Assert.Equal(48.2082, result.Latitude, precision: 4);
    }

    [Fact]
    public void TryParse_InvalidString_ReturnsFalse()
    {
        var success = GeoCoordinate.TryParse("invalid", out _);

        Assert.False(success);
    }

    [Fact]
    public void TryParse_NullInput_ReturnsFalse()
    {
        var success = GeoCoordinate.TryParse(null, out _);

        Assert.False(success);
    }

    [Fact]
    public void TryParse_EmptyString_ReturnsFalse()
    {
        var success = GeoCoordinate.TryParse("", out _);

        Assert.False(success);
    }
}
