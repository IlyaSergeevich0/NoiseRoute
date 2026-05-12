using NoiseRoute.Extensions;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace NoiseRoute.Services;

public sealed class NoiseMapGenerator
{
    private static readonly Lock Lock = new();

    public static (double[,], BitmapSource NoiseMapImage) BuildNoiseMap(
        string path,
        int expectedWidth,
        int expectedHeight)
    {
        if (!File.Exists(path))
            throw new InvalidOperationException($"Не найден файл карты шумового воздействия!");

        using var bmp = new Bitmap(path);

        if (bmp.Width != expectedWidth || bmp.Height != expectedHeight)
            throw new InvalidOperationException($"Неверный размер карты шумового воздействия: {bmp.Width}x{bmp.Height}");

        bmp.SetAllVisiblePixelsAlpha(128);

        var result = new double[bmp.Height, bmp.Width];

        Parallel.For(0, expectedHeight, (y) => {
            for (int x = 0; x < expectedWidth; x++)
            {
                Color c;

                lock (Lock)
                {
                    c = bmp.GetPixel(x, y);
                }

                var mapY = expectedHeight - y - 1;

                if (c.A == 0) // Empty
                    result[mapY, x] = 0;
                else if (c.R == 255 && c.G == 0 && c.B == 0) // Red - Residential
                    result[mapY, x] = 120;
                else if (c.R == 255 && c.G == 255 && c.B == 0) // Yellow - Industrial
                    result[mapY, x] = 60;
                else if (c.R == 0 && c.G == 0 && c.B == 255) // Blue - Prohibited (Airport)
                    result[mapY, x] = -1;
                else
                    throw new InvalidOperationException($"Unexpected pixel at ({x},{y}): {c}");
            }
        });

        return (result, bmp.ToBitmapSource());
    }
}
