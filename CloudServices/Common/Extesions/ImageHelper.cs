using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace CloudServices.Common.Extesions
{
    public static class ImageHelper
    {
        public static ImageFormat GetImageFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException(string.Format("Unable to determine file extension for fileName: {0}", fileName));
            }

            return GetImageFormatFromExtension(extension);
        }

        public static ImageFormat GetImageFormatFromExtension(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;
                case ".emf":
                    return ImageFormat.Emf;

                case ".exif":
                    return ImageFormat.Exif;

                case ".icon":
                    return ImageFormat.Icon;

                default:
                    throw new NotImplementedException();
            }
        }

        public static byte[] ConvertToBytes(Image image, string fileExtension)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, GetImageFormatFromExtension(fileExtension));

                return ms.ToArray();
            }
        }

        public static Image Crop(Image image, int x1, int y1, int x2, int y2)
        {
            if (!IsValid(image, x1, y1, x2, y2))
            {
                throw new IOException("Couldn't create croped image");
            }

            var width = x2 - x1;
            var heigh = y2 - y1;
            var currentTile = new Bitmap(width, heigh);

            currentTile.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var currentTileGraphics = Graphics.FromImage(currentTile))
            {
                var absentRectangleArea = new Rectangle(x1, y1, width, heigh);

                currentTileGraphics.DrawImage(image, 0, 0, absentRectangleArea, GraphicsUnit.Pixel);
            }

            return currentTile;
        }

        private static bool IsValid(Image image, int x1, int y1, int x2, int y2)
        {
            return (x1 >= 0 && y1 >= 0 && x2 > 0 && y2 > 0 && x2 > x1 && y2 > y1) &&
                   (x1 <= image.Width && x2 <= image.Width) &&
                   (y1 <= image.Height && y2 <= image.Height) &&
                   ((x2 - x1) <= image.Width) &&
                   ((y2 - y1) <= image.Height);
        }

        public static byte[] Resize(byte[] fileBytes, int width, int height, string fileExtension)
        {
            using (var image = Image.FromStream(new MemoryStream(fileBytes), true, true))
            {
                if (!(width <= 0 || height <= 0))
                {
                    var thumbImage = new Bitmap(width, height);
                    var rect = new Rectangle(0, 0, width, height);
                    var g = Graphics.FromImage(thumbImage);

                    g.CompositingQuality = CompositingQuality.Default;
                    g.SmoothingMode = SmoothingMode.Default;
                    g.InterpolationMode = InterpolationMode.Default;
                    g.DrawImage(image, rect);

                    return ConvertToBytes(thumbImage, fileExtension);
                }

                throw new IOException("Coudn't resize the image");
            }
        }

        public static Bitmap Rotate(Image image, float angle)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            const double pi2 = Math.PI / 2.0;

            var oldWidth = (double)image.Width;
            var oldHeight = (double)image.Height;
            var theta = angle * Math.PI / 180.0;
            var lockedTheta = theta;

            while (lockedTheta < 0.0)
            {
                lockedTheta += 2 * Math.PI;
            }

            double adjacentTop, oppositeTop;
            double adjacentBottom, oppositeBottom;

            if ((lockedTheta >= 0.0 && lockedTheta < pi2) ||
                (lockedTheta >= Math.PI && lockedTheta < (Math.PI + pi2)))
            {
                adjacentTop = Math.Abs(Math.Cos(lockedTheta)) * oldWidth;
                oppositeTop = Math.Abs(Math.Sin(lockedTheta)) * oldWidth;

                adjacentBottom = Math.Abs(Math.Cos(lockedTheta)) * oldHeight;
                oppositeBottom = Math.Abs(Math.Sin(lockedTheta)) * oldHeight;
            }
            else
            {
                adjacentTop = Math.Abs(Math.Sin(lockedTheta)) * oldHeight;
                oppositeTop = Math.Abs(Math.Cos(lockedTheta)) * oldHeight;

                adjacentBottom = Math.Abs(Math.Sin(lockedTheta)) * oldWidth;
                oppositeBottom = Math.Abs(Math.Cos(lockedTheta)) * oldWidth;
            }

            var newWidth = adjacentTop + oppositeBottom;
            var newHeight = adjacentBottom + oppositeTop;

            var nWidth = (int)Math.Ceiling(newWidth);
            var nHeight = (int)Math.Ceiling(newHeight);

            var rotatedBmp = new Bitmap(nWidth, nHeight);

            using (var g = Graphics.FromImage(rotatedBmp))
            {
                Point[] points;

                if (lockedTheta >= 0.0 && lockedTheta < pi2)
                {
                    points = new[]
                    {
                        new Point((int) oppositeBottom, 0), new Point(nWidth, (int) oppositeTop),
                        new Point(0, (int) adjacentBottom)
                    };
                }
                else if (lockedTheta >= pi2 && lockedTheta < Math.PI)
                {
                    points = new[]
                    {
                        new Point(nWidth, (int) oppositeTop), new Point((int) adjacentTop, nHeight),
                        new Point((int) oppositeBottom, 0)
                    };
                }
                else if (lockedTheta >= Math.PI && lockedTheta < (Math.PI + pi2))
                {
                    points = new[]
                    {
                        new Point((int) adjacentTop, nHeight), new Point(0, (int) adjacentBottom),
                        new Point(nWidth, (int) oppositeTop)
                    };
                }
                else
                {
                    points = new[]
                    {
                        new Point(0, (int) adjacentBottom), new Point((int) oppositeBottom, 0),
                        new Point((int) adjacentTop, nHeight)
                    };
                }
                g.DrawImage(image, points);
            }

            return rotatedBmp;
        }

        public static Image DownloadFromWeb(string path)
        {
            var fileReq = (HttpWebRequest)WebRequest.Create(path);

            var fileResp = (HttpWebResponse)fileReq.GetResponse();

            if (fileReq.ContentLength > 0)
            {
                fileResp.ContentLength = fileReq.ContentLength;
            }

            var stream = fileResp.GetResponseStream();

            return stream != null ? Image.FromStream(stream) : null;
        }
    }
}
