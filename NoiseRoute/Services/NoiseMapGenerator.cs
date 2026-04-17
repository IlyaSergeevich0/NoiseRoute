using NoiseRoute.Extensions;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace NoiseRoute.Services;

public sealed class NoiseMapGenerator
{
    public (double[,], BitmapSource NoiseMapImage) BuildNoiseMap(string path, int expectedWidth, int expectedHeight)
    {
        using var bmp = new Bitmap(path);

        if (bmp.Width != expectedWidth || bmp.Height != expectedHeight)
            throw new InvalidOperationException($"Unexpected size: {bmp.Width}x{bmp.Height}");

        var result = new double[bmp.Height, bmp.Width];

        for (int y = 0; y < expectedHeight; y++)
        {
            for (int x = 0; x < expectedWidth; x++)
            {
                Color c = bmp.GetPixel(x, y);

                var mapY = bmp.Height - y - 1;

                if (c.A == 0)
                {
                    result[mapY, x] = 0;
                }
                else if (c.R == 255 && c.G == 0 && c.B == 0)
                {
                    result[mapY, x] = 120;
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected pixel at ({x},{y}): {c}");
                }
            }
        }

        bmp.SetAllVisiblePixelsAlpha(128);

        return (result, bmp.ToBitmapSource());
    }
}
