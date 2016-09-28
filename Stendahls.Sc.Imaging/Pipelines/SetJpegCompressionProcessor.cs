using System;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Web;
using Stendahls.Sc.Imaging.Services;

namespace Stendahls.Sc.Imaging.Pipelines
{
    /// <summary>
    /// This processor sets the jpeg quality of an image and converts it into a
    /// progressive jpeg. The idea of this class is from Anders Laub (@AndersLaub)
    /// blog post about jpeg compresson levels: 
    /// https://laubplusco.net/make-sitecore-deliver-images-fits-screen/
    /// </summary>
    public class SetJpegCompressionProcessor
    {
        public void Process(GetMediaStreamPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.Options.Thumbnail || !IsJpegImageRequest(args.MediaData.MimeType))
                return;

            if (args.OutputStream == null || !args.OutputStream.AllowMemoryLoading)
                return;

            var jpegQualityQuery = WebUtil.GetQueryString("jq");
            if (string.IsNullOrEmpty(jpegQualityQuery))
                return;

            int jpegQuality;
            if (!int.TryParse(jpegQualityQuery, out jpegQuality) || jpegQuality <= 0 || jpegQuality > 100)
                return;

            try
            {
                var compressedStream = ChangeJpegCompressionLevelService.Change(args.OutputStream, jpegQuality);
                args.OutputStream = new MediaStream(compressedStream, args.MediaData.Extension, args.OutputStream.MediaItem);
            }
            catch (Exception ex)
            {
                Log.Warn("Unable to compress file", ex, this);
            }
        }

        protected bool IsJpegImageRequest(string mimeType)
        {
            return mimeType.Equals("image/jpeg", StringComparison.InvariantCultureIgnoreCase)
                   || mimeType.Equals("image/pjpeg", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
