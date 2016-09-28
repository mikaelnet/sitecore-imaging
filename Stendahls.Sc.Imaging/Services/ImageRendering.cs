using System.Globalization;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Stendahls.Sc.Imaging.Extensions;

namespace Stendahls.Sc.Imaging.Services
{
    public class ImageRendering
    {
        public MediaItem MediaItem { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }

        public int CropWidth { get; set; }
        public int CropHeight { get; set; }

        public int FixedWidth { get; set; }
        public int FixedHeight { get; set; }

        public bool AbsolutePath { get; set; } = true;

        public int JpegQuality { get; set; }


        public ImageRendering()
        {

        }

        public ImageRendering(MediaItem mediaItem)
        {
            MediaItem = mediaItem;
        }

        public ImageRendering WithMedia(MediaItem mediaItem)
        {
            MediaItem = mediaItem;
            return this;
        }

        public ImageRendering WithMaxSize(int width, int height)
        {
            MaxWidth = width;
            MaxHeight = height;
            return this;
        }

        public ImageRendering WithCrop(int width, int height)
        {
            CropWidth = width;
            CropHeight = height;
            return this;
        }

        public ImageRendering WithFixedSize(int width, int height)
        {
            FixedWidth = width;
            FixedHeight = height;
            return this;
        }

        public ImageRendering WithQuality(int quality)
        {

            if (MediaItem == null || !MediaItem.IsImage())
                return this;
            
            JpegQuality = quality;
            return this;
        }

        public ImageRendering WithAutoCrop(int width, int height)
        {
            if (MediaItem == null || !MediaItem.IsImage() || width <= 0 || height <= 0)
                return this;

            var imgWidth = MainUtil.GetInt(MediaItem.InnerItem["Width"], 0);
            var imgHeight = MainUtil.GetInt(MediaItem.InnerItem["Height"], 0);

            CropWidth = width;
            CropHeight = height;

            if (imgWidth <= 0 || imgHeight <= 0)
            {
                MaxWidth = width;
                MaxHeight = 100000;
                return this;
            }

            double sourceAspectRatio = (double)imgWidth / (double)imgHeight;
            double targetAspectRatio = (double)width / (double)height;

            if (sourceAspectRatio < targetAspectRatio)
            {
                // The +5 is just to avoid precission problems. As long as it's larger, the result will be correct
                MaxWidth = width;
                MaxHeight = (int) (width/sourceAspectRatio) + 5;
            }
            else
            {
                MaxWidth = (int)(height * sourceAspectRatio) + 5;
                MaxHeight = height;
            }

            return this;
        }

        public string GetUri()
        {
            if (MediaItem == null)
                return string.Empty;

            var isImage = MediaItem.IsImage();

            var options = new MediaUrlOptions();
            if (AbsolutePath)
            {
                options.AbsolutePath = true;
                options.VirtualFolder = "/";
            }

            if (isImage)
            {
                if (MaxWidth > 0)
                    options.MaxWidth = MaxWidth;
                if (MaxHeight > 0)
                    options.MaxHeight = MaxHeight;
                if (FixedWidth > 0)
                    options.Width = FixedWidth;
                if (FixedHeight > 0)
                    options.Height = FixedHeight;
            }

            options.LowercaseUrls = true;
            options.IncludeExtension = true;

            var mediaUrl = MediaManager.GetMediaUrl(MediaItem, options);
            bool hasParams = mediaUrl.IndexOf('?') >= 0;

            if (isImage && (CropWidth > 0 || CropHeight > 0))
            {
                mediaUrl += hasParams ? "&" : "?";
                mediaUrl += "c=1";
                hasParams = true;
                if (CropWidth > 0)
                    mediaUrl += "&cw=" + CropWidth.ToString(CultureInfo.InvariantCulture);
                if (CropHeight > 0)
                    mediaUrl += "&ch=" + CropHeight.ToString(CultureInfo.InvariantCulture);
            }

            if (MediaItem.IsJpegImage() && JpegQuality > 0 && JpegQuality <= 100)
            {
                mediaUrl += hasParams ? "&" : "?";
                mediaUrl += "jq=" + JpegQuality;
                hasParams = true;
            }

            return isImage ? HashingUtils.ProtectAssetUrl(mediaUrl) : mediaUrl;
        }
    }
}
