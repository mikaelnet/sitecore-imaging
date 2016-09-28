using System;
using Sitecore.Data.Items;

namespace Stendahls.Sc.Imaging.Extensions
{
    public static class MediaItemExtensions
    {
        public static bool IsImage(this MediaItem mediaItem)
        {
            if (mediaItem == null)
                return false;

            return mediaItem.MimeType.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsJpegImage(this MediaItem mediaItem)
        {
            if (mediaItem == null)
                return false;

            return string.Equals(mediaItem.MimeType, "image/jpg", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(mediaItem.MimeType, "image/jpeg", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string FileType(this MediaItem mediaItem)
        {
            if (string.IsNullOrWhiteSpace(mediaItem?.Extension))
                return null;

            return mediaItem.Extension.ToUpperInvariant();
        }
    }
}
