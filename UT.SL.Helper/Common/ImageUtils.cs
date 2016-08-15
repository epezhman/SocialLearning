using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT.SL.Helper
{
    public class ImageUtils
    {
        public static string[] imageExtention = { ".jpg", ".jpeg", ".bmp", ".png", ".gif" };

        /// <summary>
        /// Create Thumbnail of the picture
        /// </summary>
        /// <param name="filePath">Path to the file and name and its extension</param>
        /// <param name="newWith">thumbnail width</param>
        /// <param name="newPath">the path where thumbnail should be saved .</param>
        public static void CreateThumbnail(string filePath, int newWith, string newPath)
        {
            string extension = System.IO.Path.GetExtension(filePath);

            if (imageExtention.Contains(extension) && File.Exists(filePath))
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(filePath);
                float imgWidth = image.PhysicalDimension.Width;
                float imgHeight = image.PhysicalDimension.Height;
                float imgSize = imgHeight > imgWidth ? imgHeight : imgWidth;

                //the approximately pixel size of thumbnail image is 150 x 150 based on ratio
                float imgResize = imgSize <= newWith ? (float)1.0 : newWith / imgSize;
                imgWidth *= imgResize;
                imgHeight *= imgResize;

                System.Drawing.Image thumb1 = image.GetThumbnailImage((int)imgWidth, (int)imgHeight, delegate() { return false; }, (IntPtr)0);

                string dir = Path.GetDirectoryName(newPath);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                thumb1.Save(newPath);
            }
        }

        public static MemoryStream CreateThumbnail(MemoryStream ms, int newWith)
        {

            if (ms != null && ms.Length > 0)
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                float imgWidth = image.PhysicalDimension.Width;
                float imgHeight = image.PhysicalDimension.Height;
                float imgSize = imgHeight > imgWidth ? imgHeight : imgWidth;

                //the approximately pixel size of thumbnail image is 150 x 150 based on ratio
                float imgResize = imgSize <= newWith ? (float)1.0 : newWith / imgSize;
                imgWidth *= imgResize;
                imgHeight *= imgResize;

                System.Drawing.Image thumb1 = image.GetThumbnailImage((int)imgWidth, (int)imgHeight, delegate() { return false; }, (IntPtr)0);

                MemoryStream outms = new MemoryStream();

                thumb1.Save(outms, System.Drawing.Imaging.ImageFormat.Jpeg);

                return outms;
            }
            return null;
        }

        public static bool IsImageFileName(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName).ToLower();

            if (imageExtention.Contains(ext))
                return true;

            return false;
        }
    }
}
