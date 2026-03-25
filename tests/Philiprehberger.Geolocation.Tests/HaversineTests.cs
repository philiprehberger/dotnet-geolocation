using Xunit;
using Philiprehberger.Geolocation;

namespace Philiprehberger.Geolocation.Tests;

public class HaversineTests
{
    [Fact]
    public void Calculate_SamePoint_ReturnsZero()
    {
        var coord = new GeoCoordinate(48.2082, 16.3738);

        var distance = Haversine.Calculate(coord, coord);

        Assert.Equal(0.0, distance, precision: 5);
    }

    [Fact]
    public void Calculate_ViennaToLondon_ReturnsApproximateDistance()
    {
        var vienna = new GeoCoordinate(48.2082, 16.3738);
        var london = new GeoCoordinate(51.5074, -0.1278);

        var distance = Haversine.Calculate(vienna, london);

        Assert.InRange(distance, 1230.0, 1250.0);
    }

    [Fact]
    public void Calculate_IsCommutative()
    {
        var a = new GeoCoordinate(40.7128, -74.0060);
        var b = new GeoCoordinate(34.0522, -118.2437);

        var distanceAB = Haversine.Calculate(a, b);
        var distanceBA = Haversine.Calculate(b, a);

        Assert.Equal(distanceAB, distanceBA, precision: 10);
    }
}
