using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Web;
using Stendahls.Sc.Imaging.Services;

namespace Stendahls.Sc.Imaging.Pipelines
{
    /// <summary>
    /// Crops images according to given size. This class is essentially the same as 
    /// the one provided by Anders Laub (@AndersLaub) and is described in this post: 
    /// https://laubplusco.net/make-sitecore-deliver-images-fits-screen-part-2/
    /// </summary>
    public class ImageCropProcessor
    {
        public IEnumerable<string> ValidMimeTypes
        {
            get
            {
                var validMimetypes = Settings.GetSetting("ImageCrop.MimeTypes", "image/jpeg|image/pjpeg|image/png|image/gif|image/tiff|image/bmp");
                return validMimetypes.Split(new[] { ',', '|', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public void Process(GetMediaStreamPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.Options.Thumbnail || !IsValidImageRequest(args.MediaData.MimeType))
                return;

            if (args.OutputStream == null || !args.OutputStream.AllowMemoryLoading)
                return;

            var cropKey = GetQueryOrCustomOption("c", args.Options.CustomOptions);
            if (string.IsNullOrEmpty(cropKey))
                return;

            var cropWidthOption = GetQueryOrCustomOption("cw", args.Options.CustomOptions);
            var cropHeightOption = GetQueryOrCustomOption("ch", args.Options.CustomOptions);

            if (string.IsNullOrEmpty(cropWidthOption) && string.IsNullOrEmpty(cropHeightOption))
                return;

            var transformationOptions = args.Options.GetTransformationOptions();
            if (!transformationOptions.ContainsResizing())
                return;

            int cropWidth;
            if (!int.TryParse(cropWidthOption, out cropWidth))
                cropWidth = args.Options.Width;
            int cropHeight;
            if (!int.TryParse(cropHeightOption, out cropHeight))
                cropHeight = args.Options.Height;

            var croppedStream = CropImageService.CenterCrop(args.OutputStream.Stream, cropWidth, cropHeight, GetImageFormat(args.MediaData.MimeType.ToLower()));
            args.OutputStream = new MediaStream(croppedStream, args.MediaData.Extension, args.OutputStream.MediaItem);
        }

        private ImageFormat GetImageFormat(string mimeType)
        {
            switch (mimeType)
            {
                case "image/jpeg":
                    return ImageFormat.Jpeg;
                case "image/pjpeg":
                    return ImageFormat.Jpeg;
                case "image/png":
                    return ImageFormat.Png;
                case "image/gif":
                    return ImageFormat.Gif;
                case "image/tiff":
                    return ImageFormat.Tiff;
                case "image/bmp":
                    return ImageFormat.Bmp;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        protected bool IsValidImageRequest(string mimeType)
        {
            return ValidMimeTypes.Any(v => v.Equals(mimeType, StringComparison.InvariantCultureIgnoreCase));
        }

        protected string GetQueryOrCustomOption(string key, StringDictionary customOptions)
        {
            var value = WebUtil.GetQueryString(key);
            return string.IsNullOrEmpty(value) ? customOptions[key] : value;
        }
    }
}
