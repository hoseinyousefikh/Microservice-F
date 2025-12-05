using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Catalog_Service.src.CrossCutting.Utilities
{
    public static class ImageProcessor
    {
        public static async Task<byte[]> ResizeImageAsync(byte[] imageData, int maxWidth, int maxHeight, int quality = 75)
        {
            using (var inputStream = new MemoryStream(imageData))
            using (var outputStream = new MemoryStream())
            {
                using (var image = Image.FromStream(inputStream))
                {
                    var ratioX = (double)maxWidth / image.Width;
                    var ratioY = (double)maxHeight / image.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    var newWidth = (int)(image.Width * ratio);
                    var newHeight = (int)(image.Height * ratio);

                    using (var thumbnail = new Bitmap(newWidth, newHeight))
                    {
                        using (var graphic = Graphics.FromImage(thumbnail))
                        {
                            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphic.SmoothingMode = SmoothingMode.HighQuality;
                            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            graphic.DrawImage(image, 0, 0, newWidth, newHeight);
                        }

                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                        var imageFormat = GetImageFormat(image.RawFormat);
                        var encoder = GetEncoder(imageFormat);
                        thumbnail.Save(outputStream, encoder, encoderParameters);
                    }
                }

                return outputStream.ToArray();
            }
        }

        public static async Task<byte[]> ConvertToJpegAsync(byte[] imageData, int quality = 75)
        {
            using (var inputStream = new MemoryStream(imageData))
            using (var outputStream = new MemoryStream())
            {
                using (var image = Image.FromStream(inputStream))
                {
                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                    var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                    image.Save(outputStream, jpegEncoder, encoderParameters);
                }

                return outputStream.ToArray();
            }
        }

        public static bool IsImage(byte[] imageData)
        {
            try
            {
                using (var stream = new MemoryStream(imageData))
                {
                    using (var image = Image.FromStream(stream))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static ImageFormat GetImageFormat(ImageFormat rawFormat)
        {
            if (rawFormat.Equals(ImageFormat.Jpeg) || rawFormat.Equals(ImageFormat.Jpeg))
                return ImageFormat.Jpeg;
            if (rawFormat.Equals(ImageFormat.Png))
                return ImageFormat.Png;
            if (rawFormat.Equals(ImageFormat.Gif))
                return ImageFormat.Gif;
            if (rawFormat.Equals(ImageFormat.Bmp))
                return ImageFormat.Bmp;

            return ImageFormat.Jpeg;
        }
    }
}