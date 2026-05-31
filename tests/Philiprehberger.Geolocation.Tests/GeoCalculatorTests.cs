using Xunit;
using Philiprehberger.Geolocation;

namespace Philiprehberger.Geolocation.Tests;

public class GeoCalculatorTests
{
    // --- Midpoint tests ---

    [Fact]
    public void Midpoint_SamePoint_ReturnsSamePoint()
    {
        var point = new GeoCoordinate(48.2082, 16.3738);

        var mid = GeoCalculator.Midpoint(point, point);

        Assert.Equal(point.Latitude, mid.Latitude, precision: 4);
        Assert.Equal(point.Longitude, mid.Longitude, precision: 4);
    }

    [Fact]
    public void Midpoint_SymmetricPoints_ReturnsCenter()
    {
        var north = new GeoCoordinate(10.0, 0.0);
        var south = new GeoCoordinate(-10.0, 0.0);

        var mid = GeoCalculator.Midpoint(north, south);

        Assert.Equal(0.0, mid.Latitude, precision: 4);
        Assert.Equal(0.0, mid.Longitude, precision: 4);
    }

    [Fact]
    public void Midpoint_ViennaAndBerlin_ReturnsApproximateCenter()
    {
        var vienna = new GeoCoordinate(48.2082, 16.3738);
        var berlin = new GeoCoordinate(52.5200, 13.4050);

        var mid = GeoCalculator.Midpoint(vienna, berlin);

        // Midpoint should be roughly between the two latitudes and longitudes
        Assert.InRange(mid.Latitude, 50.0, 51.0);
        Assert.InRange(mid.Longitude, 14.5, 15.5);
    }

    [Fact]
    public void Midpoint_IsCommutative()
    {
        var a = new GeoCoordinate(40.7128, -74.0060);
        var b = new GeoCoordinate(34.0522, -118.2437);

        var midAB = GeoCalculator.Midpoint(a, b);
        var midBA = GeoCalculator.Midpoint(b, a);

        Assert.Equal(midAB.Latitude, midBA.Latitude, precision: 10);
        Assert.Equal(midAB.Longitude, midBA.Longitude, precision: 10);
    }

    // --- Bearing tests ---

    [Fact]
    public void Bearing_DueNorth_ReturnsZero()
    {
        var from = new GeoCoordinate(0.0, 0.0);
        var to = new GeoCoordinate(10.0, 0.0);

        var bearing = GeoCalculator.Bearing(from, to);

        Assert.Equal(0.0, bearing, precision: 1);
    }

    [Fact]
    public void Bearing_DueEast_Returns90()
    {
        var from = new GeoCoordinate(0.0, 0.0);
        var to = new GeoCoordinate(0.0, 10.0);

        var bearing = GeoCalculator.Bearing(from, to);

        Assert.Equal(90.0, bearing, precision: 1);
    }

    [Fact]
    public void Bearing_DueSouth_Returns180()
    {
        var from = new GeoCoordinate(10.0, 0.0);
        var to = new GeoCoordinate(0.0, 0.0);

        var bearing = GeoCalculator.Bearing(from, to);

        Assert.Equal(180.0, bearing, precision: 1);
    }

    [Fact]
    public void Bearing_DueWest_Returns270()
    {
        var from = new GeoCoordinate(0.0, 10.0);
        var to = new GeoCoordinate(0.0, 0.0);

        var bearing = GeoCalculator.Bearing(from, to);

        Assert.Equal(270.0, bearing, precision: 1);
    }

    [Fact]
    public void Bearing_ReturnsValueBetween0And360()
    {
        var from = new GeoCoordinate(48.2082, 16.3738);
        var to = new GeoCoordinate(52.5200, 13.4050);

        var bearing = GeoCalculator.Bearing(from, to);

        Assert.InRange(bearing, 0.0, 360.0);
    }

    // --- ClosestTo tests ---

    [Fact]
    public void ClosestTo_SingleCandidate_ReturnsThatCandidate()
    {
        var target = new GeoCoordinate(48.2082, 16.3738);
        var candidates = new[] { new GeoCoordinate(52.5200, 13.4050) };

        var result = GeoCalculator.ClosestTo(target, candidates).ToList();

        Assert.Single(result);
        Assert.Equal(candidates[0], result[0]);
    }

    [Fact]
    public void ClosestTo_MultipleCandidates_ReturnsNearest()
    {
        var vienna = new GeoCoordinate(48.2082, 16.3738);
        var bratislava = new GeoCoordinate(48.1486, 17.1077); // ~55 km
        var berlin = new GeoCoordinate(52.5200, 13.4050);     // ~524 km
        var london = new GeoCoordinate(51.5074, -0.1278);     // ~1237 km

        var result = GeoCalculator.ClosestTo(vienna, new[] { london, berlin, bratislava }).ToList();

        Assert.Single(result);
        Assert.Equal(bratislava, result[0]);
    }

    [Fact]
    public void ClosestTo_CountGreaterThanOne_ReturnsMultipleOrdered()
    {
        var vienna = new GeoCoordinate(48.2082, 16.3738);
        var bratislava = new GeoCoordinate(48.1486, 17.1077);
        var berlin = new GeoCoordinate(52.5200, 13.4050);
        var london = new GeoCoordinate(51.5074, -0.1278);

        var result = GeoCalculator.ClosestTo(vienna, new[] { london, berlin, bratislava }, count: 2).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(bratislava, result[0]);
        Assert.Equal(berlin, result[1]);
    }

    [Fact]
    public void ClosestTo_CountExceedsCandidates_ReturnsAllCandidates()
    {
        var target = new GeoCoordinate(0.0, 0.0);
        var candidates = new[] { new GeoCoordinate(1.0, 1.0), new GeoCoordinate(2.0, 2.0) };

        var result = GeoCalculator.ClosestTo(target, candidates, count: 10).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ClosestTo_NullCandidates_ThrowsArgumentNullException()
    {
        var target = new GeoCoordinate(0.0, 0.0);

        Assert.Throws<ArgumentNullException>(() => GeoCalculator.ClosestTo(target, null!));
    }

    [Fact]
    public void ClosestTo_CountLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        var target = new GeoCoordinate(0.0, 0.0);
        var candidates = new[] { new GeoCoordinate(1.0, 1.0) };

        Assert.Throws<ArgumentOutOfRangeException>(() => GeoCalculator.ClosestTo(target, candidates, count: 0));
    }

    [Fact]
    public void DestinationPoint_TravelsAlongBearing_DistanceMatches()
    {
        var start = new GeoCoordinate(48.2082, 16.3738);
        var destination = GeoCalculator.DestinationPoint(start, bearingDegrees: 90.0, distanceKm: 100.0);

        var measured = Geo.Distance(start, destination);
        Assert.InRange(measured, 99.0, 101.0);
    }

    [Fact]
    public void DestinationPoint_NegativeDistance_ThrowsArgumentOutOfRangeException()
    {
        var start = new GeoCoordinate(0.0, 0.0);
        Assert.Throws<ArgumentOutOfRangeException>(() => GeoCalculator.DestinationPoint(start, 0.0, -1.0));
    }

    [Fact]
    public void IntermediatePoint_FractionZero_ReturnsStart()
    {
        var a = new GeoCoordinate(48.2082, 16.3738);
        var b = new GeoCoordinate(52.5200, 13.4050);

        var result = GeoCalculator.IntermediatePoint(a, b, 0.0);

        Assert.Equal(a.Latitude, result.Latitude, 5);
        Assert.Equal(a.Longitude, result.Longitude, 5);
    }

    [Fact]
    public void IntermediatePoint_FractionOne_ReturnsEnd()
    {
        var a = new GeoCoordinate(48.2082, 16.3738);
        var b = new GeoCoordinate(52.5200, 13.4050);

        var result = GeoCalculator.IntermediatePoint(a, b, 1.0);

        Assert.Equal(b.Latitude, result.Latitude, 5);
        Assert.Equal(b.Longitude, result.Longitude, 5);
    }

    [Fact]
    public void IntermediatePoint_FractionHalf_RoughlyMatchesMidpoint()
    {
        var a = new GeoCoordinate(48.2082, 16.3738);
        var b = new GeoCoordinate(52.5200, 13.4050);

        var halfway = GeoCalculator.IntermediatePoint(a, b, 0.5);
        var midpoint = GeoCalculator.Midpoint(a, b);

        Assert.Equal(midpoint.Latitude, halfway.Latitude, 4);
        Assert.Equal(midpoint.Longitude, halfway.Longitude, 4);
    }

    [Fact]
    public void IntermediatePoint_FractionOutsideRange_ThrowsArgumentOutOfRangeException()
    {
        var a = new GeoCoordinate(0.0, 0.0);
        var b = new GeoCoordinate(1.0, 1.0);

        Assert.Throws<ArgumentOutOfRangeException>(() => GeoCalculator.IntermediatePoint(a, b, -0.1));
        Assert.Throws<ArgumentOutOfRangeException>(() => GeoCalculator.IntermediatePoint(a, b, 1.1));
    }
}
