using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Stendahls.Sc.Imaging.Services
{
    public class CropImageService
    {
        public static Stream CenterCrop(Stream imageStream, int width, int height, ImageFormat format)
        {
            var bitmap = new Bitmap(imageStream);
            if (bitmap.Width == width && bitmap.Height == height)
                return imageStream;

            var cropWidth = width < bitmap.Width && width != 0 ? width : bitmap.Width;
            var cropHeight = height < bitmap.Height && height != 0 ? height : bitmap.Height;
            var x = cropWidth == bitmap.Width ? 0 : (bitmap.Width / 2) - (cropWidth / 2);
            var y = cropHeight == bitmap.Width ? 0 : (bitmap.Height / 2) - (cropHeight / 2);

            var croppedBitmap = CropImage(bitmap, x, y, cropWidth, cropHeight);
            var memoryStream = new MemoryStream();
            croppedBitmap.Save(memoryStream, format);
            return memoryStream;
        }

        public static Bitmap CropImage(Bitmap originalImage, int x, int y, int width, int height)
        {
            if (x < 0 || ((x + width) > originalImage.Width) || (y < 0) || ((y + height) > originalImage.Height))
                return originalImage;
            var rectangle = new Rectangle(x, y, width, height);
            var croppedImage = new Bitmap(rectangle.Width, rectangle.Height);
            using (var g = Graphics.FromImage(croppedImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.DrawImage(originalImage, new Rectangle(0, 0, croppedImage.Width, croppedImage.Height), rectangle,
                    GraphicsUnit.Pixel);
            }
            return croppedImage;
        }
    }
}