using NoiseRoute.Models;

namespace NoiseRoute.Services;

public sealed class MapGenerator
{
    private readonly Random _random = new();

    public List<MapZone> GenerateZones(int width, int height)
    {
        const int ResidentalsCount = 4;

        var baseX = 200;
        var baseY = 200;
        var zones = new List<MapZone>();

        // Residentals
        for (var i = 0; i < ResidentalsCount; i += 1)
        {
            var radius = 50 + _random.NextDouble() * 200;
            var x = _random.Next(baseX, width - 200);
            var y = _random.Next(baseY, height - 200);

            zones.Add(new MapZone {
                X = x,
                Y = y,
                Type = MapZoneType.Residential,
                Radius = radius,
                SplRef = 40 + 20 * Math.Log10(radius)
            });
        }

        return zones;
    }

}
