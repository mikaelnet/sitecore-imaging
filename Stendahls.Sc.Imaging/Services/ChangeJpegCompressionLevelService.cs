using System.IO;
using ImageMagick;
using Sitecore.Resources.Media;

namespace Stendahls.Sc.Imaging.Services
{
    public class ChangeJpegCompressionLevelService
    {
        public static Stream Change(MediaStream mediaStream, int jpegQuality)
        {
            var dataFolder = Sitecore.Configuration.Settings.DataFolder;
            var cacheFolder = Path.Combine(dataFolder, @"media\cache");
            var tempFolder = Path.Combine(dataFolder, @"media\temp");
            Directory.CreateDirectory(cacheFolder);
            Directory.CreateDirectory(tempFolder);

            MagickAnyCPU.CacheDirectory = cacheFolder;
            MagickNET.SetTempDirectory(tempFolder);

            var image = new MagickImage(mediaStream.Stream)
            {
                Format = MagickFormat.Pjpeg,
                Interlace = Interlace.Plane,
                Quality = jpegQuality
            };
            //image.GaussianBlur(0.1f, 0.05f);
            image.Strip();
            var stream = new MemoryStream();
            image.Write(stream);
            return stream;
        }
    }
}