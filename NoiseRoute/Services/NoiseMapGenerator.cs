using NoiseRoute.Models;

namespace NoiseRoute.Services;

public sealed class NoiseMapGenerator
{
    public double[,] BuildNoiseMap(List<MapZone> mapZones, int width, int height)
    {
        var noise = new double[height, width];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var maxNoise = 0.0;

                foreach (var zone in mapZones)
                {
                    var zoneNoise = zone.GetNoiseAt(x, y);

                    if (maxNoise < zoneNoise)
                        maxNoise = zoneNoise;
                }

                noise[y, x] = maxNoise;
            }

        return noise;
    }
}
