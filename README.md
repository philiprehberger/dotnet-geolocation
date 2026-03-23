# Philiprehberger.Geolocation

[![CI](https://github.com/philiprehberger/dotnet-geolocation/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-geolocation/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.Geolocation.svg)](https://www.nuget.org/packages/Philiprehberger.Geolocation)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-geolocation)](LICENSE)

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
var nearby = Geo.Filter(cities, vienna, radiusKm: 400); // Munich only
```

## API

### `Geo`

| Method | Description |
|--------|-------------|
| `Distance(GeoCoordinate coord1, GeoCoordinate coord2, DistanceUnit unit = Kilometers)` | Calculates the great-circle distance using the Haversine formula. |
| `BoundingBox(GeoCoordinate center, double radiusKm)` | Returns min/max coordinates for a bounding box around the center point. |
| `IsWithinRadius(GeoCoordinate point, GeoCoordinate center, double radiusKm)` | Checks if a point is within the specified radius. |
| `Filter(IEnumerable<GeoCoordinate> points, GeoCoordinate center, double radiusKm)` | Returns all points within the specified radius. |

### `GeoCoordinate`

Readonly record struct with `Latitude` (-90 to 90) and `Longitude` (-180 to 180) validation.

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

## License

MIT
