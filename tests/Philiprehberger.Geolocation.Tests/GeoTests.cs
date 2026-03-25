using Xunit;
using Philiprehberger.Geolocation;

namespace Philiprehberger.Geolocation.Tests;

public class GeoTests
{
    [Fact]
    public void Distance_DefaultUnit_ReturnsKilometers()
    {
        var vienna = new GeoCoordinate(48.2082, 16.3738);
        var london = new GeoCoordinate(51.5074, -0.1278);

        var km = Geo.Distance(vienna, london);

        Assert.InRange(km, 1230.0, 1250.0);
    }

    [Fact]
    public void Distance_Miles_ReturnsSmallerValue()
    {
        var a = new GeoCoordinate(48.2082, 16.3738);
        var b = new GeoCoordinate(51.5074, -0.1278);

        var km = Geo.Distance(a, b, DistanceUnit.Kilometers);
        var miles = Geo.Distance(a, b, DistanceUnit.Miles);

        Assert.True(miles < km);
    }

    [Fact]
    public void BoundingBox_NegativeRadius_ThrowsArgumentOutOfRangeException()
    {
        var center = new GeoCoordinate(48.2082, 16.3738);

        Assert.Throws<ArgumentOutOfRangeException>(() => Geo.BoundingBox(center, -1.0));
    }

    [Fact]
    public void BoundingBox_ValidRadius_ReturnsMinLessThanMax()
    {
        var center = new GeoCoordinate(48.2082, 16.3738);

        var (min, max) = Geo.BoundingBox(center, 100.0);

        Assert.True(min.Latitude < max.Latitude);
        Assert.True(min.Longitude < max.Longitude);
    }

    [Fact]
    public void IsWithinRadius_PointInside_ReturnsTrue()
    {
        var center = new GeoCoordinate(48.2082, 16.3738);
        var nearby = new GeoCoordinate(48.21, 16.38);

        Assert.True(Geo.IsWithinRadius(nearby, center, 10.0));
    }

    [Fact]
    public void IsWithinRadius_PointOutside_ReturnsFalse()
    {
        var center = new GeoCoordinate(48.2082, 16.3738);
        var far = new GeoCoordinate(51.5074, -0.1278);

        Assert.False(Geo.IsWithinRadius(far, center, 10.0));
    }

    [Fact]
    public void Filter_ReturnsOnlyPointsWithinRadius()
    {
        var center = new GeoCoordinate(48.2082, 16.3738);
        var points = new[]
        {
            new GeoCoordinate(48.21, 16.38),
            new GeoCoordinate(51.5074, -0.1278),
            new GeoCoordinate(48.20, 16.37)
        };

        var result = Geo.Filter(points, center, 10.0).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Filter_NullPoints_ThrowsArgumentNullException()
    {
        var center = new GeoCoordinate(48.2082, 16.3738);

        Assert.Throws<ArgumentNullException>(() => Geo.Filter(null!, center, 10.0).ToList());
    }
}
