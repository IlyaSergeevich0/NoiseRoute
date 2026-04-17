using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace NoiseRoute.Extensions;

public static class BitmapExtensions
{
    public static void SetAllVisiblePixelsAlpha(this Bitmap bmp, byte alpha)
    {
        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        var data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

        try
        {
            int bytes = Math.Abs(data.Stride) * bmp.Height;
            byte[] buffer = new byte[bytes];

            Marshal.Copy(data.Scan0, buffer, 0, bytes);

            for (int i = 0; i < buffer.Length; i += 4)
            {
                if (buffer[i + 3] != 0)
                    buffer[i + 3] = alpha;
            }

            Marshal.Copy(buffer, 0, data.Scan0, bytes);
        }
        finally
        {
            bmp.UnlockBits(data);
        }
    }

    public static BitmapSource ToBitmapSource(this Bitmap bitmap)
    {
        IntPtr hBitmap = bitmap.GetHbitmap();
        try
        {
            var source = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            source.Freeze();
            return source;
        }
        finally
        {
            DeleteObject(hBitmap);
        }
    }

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool DeleteObject(IntPtr hObject);
}
