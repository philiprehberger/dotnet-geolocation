# Philiprehberger.Geolocation

[![CI](https://github.com/philiprehberger/dotnet-geolocation/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-geolocation/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.Geolocation.svg)](https://www.nuget.org/packages/Philiprehberger.Geolocation)
[![Last updated](https://img.shields.io/github/last-commit/philiprehberger/dotnet-geolocation)](https://github.com/philiprehberger/dotnet-geolocation/commits/main)

Calculate distances between coordinates, find points within radius, and compute bounding boxes.

## Installation

```bash
dotnet add package Philiprehberger.Geolocation
```

## Usage

```csharp
using Philiprehberger.Geolocation;

var vienna = new GeoCoordinate(48.2082, 16.3738);
var berlin = new GeoCoordinate(52.5200, 13.4050);

// Calculate distance
double km = Geo.Distance(vienna, berlin); // ~524 km
double miles = Geo.Distance(vienna, berlin, DistanceUnit.Miles);

// Check if within radius
bool nearby = Geo.IsWithinRadius(berlin, vienna, radiusKm: 600); // true

// Get bounding box
var (min, max) = Geo.BoundingBox(vienna, radiusKm: 50);

// Filter points within radius
var cities = new[] { berlin, new GeoCoordinate(48.1351, 11.5820) }; // Berlin, Munich
var nearbyCities = Geo.Filter(cities, vienna, radiusKm: 400); // Munich only
```

### Midpoint

```csharp
using Philiprehberger.Geolocation;

var vienna = new GeoCoordinate(48.2082, 16.3738);
var berlin = new GeoCoordinate(52.5200, 13.4050);

var midpoint = GeoCalculator.Midpoint(vienna, berlin);
// midpoint.Latitude ~ 50.37, midpoint.Longitude ~ 14.92
```

### Bearing

```csharp
using Philiprehberger.Geolocation;

var vienna = new GeoCoordinate(48.2082, 16.3738);
var berlin = new GeoCoordinate(52.5200, 13.4050);

double bearing = GeoCalculator.Bearing(vienna, berlin); // ~336° (NNW)
```

### Closest-N Query

```csharp
using Philiprehberger.Geolocation;

var vienna = new GeoCoordinate(48.2082, 16.3738);
var cities = new[]
{
    new GeoCoordinate(48.1486, 17.1077), // Bratislava
    new GeoCoordinate(52.5200, 13.4050), // Berlin
    new GeoCoordinate(51.5074, -0.1278), // London
};

var closest = GeoCalculator.ClosestTo(vienna, cities);          // Bratislava
var top2 = GeoCalculator.ClosestTo(vienna, cities, count: 2);   // Bratislava, Berlin
```

### DMS Coordinate Parsing

```csharp
using Philiprehberger.Geolocation;

// Parse DMS notation
var coord = GeoCoordinate.Parse("40°26'46\"N 79°58'56\"W");
// coord.Latitude ~ 40.446, coord.Longitude ~ -79.982

// Parse decimal notation
var vienna = GeoCoordinate.Parse("48.2082, 16.3738");

// Safe parsing
if (GeoCoordinate.TryParse("48.2082, 16.3738", out var result))
{
    Console.WriteLine($"{result.Latitude}, {result.Longitude}");
}
```

## API

### `Geo`

| Method | Description |
|--------|-------------|
| `Distance(GeoCoordinate coord1, GeoCoordinate coord2, DistanceUnit unit = Kilometers)` | Calculates the great-circle distance using the Haversine formula. |
| `BoundingBox(GeoCoordinate center, double radiusKm)` | Returns min/max coordinates for a bounding box around the center point. |
| `IsWithinRadius(GeoCoordinate point, GeoCoordinate center, double radiusKm)` | Checks if a point is within the specified radius. |
| `Filter(IEnumerable<GeoCoordinate> points, GeoCoordinate center, double radiusKm)` | Returns all points within the specified radius. |

### `GeoCalculator`

| Method | Description |
|--------|-------------|
| `Midpoint(GeoCoordinate a, GeoCoordinate b)` | Calculates the geographic midpoint using the spherical midpoint formula. |
| `Bearing(GeoCoordinate from, GeoCoordinate to)` | Calculates the initial bearing in degrees (0-360) using the forward azimuth formula. |
| `ClosestTo(GeoCoordinate target, IEnumerable<GeoCoordinate> candidates, int count = 1)` | Finds the K nearest coordinates to the target, ordered by distance. |

### `GeoCoordinate`

Readonly record struct with `Latitude` (-90 to 90) and `Longitude` (-180 to 180) validation.

| Method | Description |
|--------|-------------|
| `Parse(string input)` | Parses a decimal or DMS coordinate string. Throws `FormatException` on invalid input. |
| `TryParse(string? input, out GeoCoordinate result)` | Attempts to parse a decimal or DMS coordinate string. Returns `false` on failure. |

### `DistanceUnit`

Enum: `Kilometers`, `Miles`, `Meters`, `NauticalMiles`.

### `Haversine`

| Method | Description |
|--------|-------------|
| `Calculate(GeoCoordinate coord1, GeoCoordinate coord2)` | Returns distance in kilometers using the Haversine formula. |

## Development

```bash
dotnet build src/Philiprehberger.Geolocation.csproj --configuration Release
```

## Support

If you find this project useful:

⭐ [Star the repo](https://github.com/philiprehberger/dotnet-geolocation)

🐛 [Report issues](https://github.com/philiprehberger/dotnet-geolocation/issues?q=is%3Aissue+is%3Aopen+label%3Abug)

💡 [Suggest features](https://github.com/philiprehberger/dotnet-geolocation/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)

❤️ [Sponsor development](https://github.com/sponsors/philiprehberger)

🌐 [All Open Source Projects](https://philiprehberger.com/open-source-packages)

💻 [GitHub Profile](https://github.com/philiprehberger)

🔗 [LinkedIn Profile](https://www.linkedin.com/in/philiprehberger)

## License

[MIT](LICENSE)
