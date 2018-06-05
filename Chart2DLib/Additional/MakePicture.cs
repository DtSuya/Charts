using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Readearth.Chart2D.Additional
{
    public class MakePicture
    {
        //导出图片
        public static bool OutTheImageToPic(Image bitmap, string fileFullName)
        {
            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                fileFullName = path + "Chart2D.bmp";
                int i = 1;
                while (File.Exists(fileFullName))
                {
                    fileFullName = fileFullName.Replace(".bmp", i + ".bmp");
                    i++;
                }
            }
            bool isSave = true;
            System.Drawing.Imaging.ImageFormat imgformat = null;
            string fileExtName = fileFullName.Substring(fileFullName.LastIndexOf(".") + 1).ToString();
            if (fileExtName != "")
            {
                switch (fileExtName)
                {
                    case "jpg":
                    case "jpeg":
                        imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                        break;
                    case "bmp":
                        imgformat = System.Drawing.Imaging.ImageFormat.Bmp;
                        break;
                    case "gif":
                        imgformat = System.Drawing.Imaging.ImageFormat.Gif;
                        break;
                    case "png":
                        imgformat = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                    case "tif":
                    case "tiff":
                        imgformat = System.Drawing.Imaging.ImageFormat.Tiff;
                        break;
                    default:
                        //MessageBox.Show("只能存取为: jpg,bmp,gif,png 格式");
                        isSave = false;
                        break;
                }
            }
            //默认保存为JPG格式  
            if (imgformat == null)
            {
                imgformat = System.Drawing.Imaging.ImageFormat.Bmp;
            }
            if (isSave)
            {
                try
                {
                    bitmap.Save(fileFullName, imgformat);
                    return true;
                }
                catch
                { return false; }
            }
            else
                return false;
        }
        

        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="originalBmp">源图对象</param>
        /// <param name="newWidth">目标图宽度</param>
        /// <param name="newHeight">目标图高度</param>
        /// <param name="Mode">生成目标图的方式</param>    
        public static Bitmap ResizeImage(Bitmap originalBmp, int newWidth, int newHeight, string Mode)
        {
            int targetWidth = newWidth;
            int targetHeight = newHeight;

            int x = 0;
            int y = 0;
            int ow = originalBmp.Width;
            int oh = originalBmp.Height;

            switch (Mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    targetHeight = originalBmp.Height * newWidth / originalBmp.Width;
                    break;
                case "H"://指定高，宽按比例
                    targetWidth = originalBmp.Width * newHeight / originalBmp.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalBmp.Width / (double)originalBmp.Height > (double)targetWidth / (double)targetHeight)
                    {
                        oh = originalBmp.Height;
                        ow = originalBmp.Height * targetWidth / targetHeight;
                        y = 0;
                        x = (originalBmp.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalBmp.Width;
                        oh = originalBmp.Width * newHeight / targetWidth;
                        x = 0;
                        y = (originalBmp.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建目标Bitmap
            Bitmap bitmap = new Bitmap(targetWidth, targetHeight);
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置插值方法和平滑程度
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布，以透明背景色填充
            g.Clear(Color.Transparent);
            //按指定大小绘制原图的指定部分
            g.DrawImage(originalBmp, new Rectangle(0, 0, targetWidth, targetHeight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);

            originalBmp.Dispose();
            g.Dispose();

            return bitmap;
        }
    }
}
