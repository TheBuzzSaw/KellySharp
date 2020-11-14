using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace KonsoleApp
{
    static class ImageExtensions
    {
        public static void PaintBox<T>(
            this Image<T> image,
            int topLeftX,
            int topLeftY,
            int width,
            int height,
            T pixel) where T : unmanaged, IPixel<T>
        {
            for (int i = 0; i < height; ++i)
            for (int j = 0; j < width; ++j)
            {
                image[topLeftX + j, topLeftY + i] = pixel;
            }
        }

        public static byte InterpolateTo(this byte start, byte end, double t)
        {
            var result = start + (end - start) * t;
            return (byte)result;
        }

        public static Rgba32 InterpolateTo(this Rgba32 start, Rgba32 end, double t)
        {
            return new Rgba32(
                start.R.InterpolateTo(end.R, t),
                start.G.InterpolateTo(end.G, t),
                start.B.InterpolateTo(end.B, t),
                start.A.InterpolateTo(end.A, t));
        }
    }
}