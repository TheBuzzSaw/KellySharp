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
    }
}