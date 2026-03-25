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
}
