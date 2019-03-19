namespace FinnZan.Utilities
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
#if !DOTNET_4
    using System.Web.Script.Serialization;
#endif
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class CommonTools
    {
#region File =====================================================

        public static string GetMIME(string sRequestedFile)
        {
            String sFileExt = "";
            sRequestedFile = sRequestedFile.ToLower();
            int iStartPos = sRequestedFile.LastIndexOf(".") + 1;
            sFileExt = sRequestedFile.Substring(iStartPos);

            try
            {
                if (sFileExt.Equals("jpg") || sFileExt.Equals("JPG"))
                {
                    return "image/jpeg";
                }
                else if (sFileExt.Equals("mp4"))
                {
                    return "video/mp4";
                }
                else if (sFileExt.Equals("mp3"))
                {
                    return "audio/mpeg";
                }
                else if (sFileExt.Equals("3gp"))
                {
                    return "video/3gpp";
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
            }
            return "";
        }

        public static bool IsUnsafeFileType(string fileName)
        {
            //  The list is copied from Gmail blocked file types:
            //  https://support.google.com/mail/answer/6590?hl=en
            string[] ListBlockedType = { ".ade", ".adp", ".bat", ".chm", ".cmd", ".com", ".cpl", ".exe", ".hta", ".ins", ".isp", ".jar", ".jse",
                                          ".lib", ".lnk", ".mde", ".msc", ".msp", ".mst", ".pif", ".scr", ".sct", ".shb", ".sys", ".vb", ".vbe",
                                          ".vbs", ".vxd", ".wsc", ".wsf", ".wsh"
                                        };

            foreach (string strBlockedType in ListBlockedType)
            {
                if (fileName.ToLower().EndsWith(strBlockedType))
                {
                    return true;
                }
            }

            return false;
        }

        public static byte[] BitmapImageToByteArray(BitmapSource bmp)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            MemoryStream ms = new MemoryStream();
            encoder.Save(ms);
            return ms.ToArray();
        }

        public static BitmapSource ByteArrayToBitmapImage(byte[] data)
        {
            try
            {
                var ms = new System.IO.MemoryStream(data);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return null;
            }
        }

        public static BitmapSource ScaleBitmapImage(BitmapSource src, int targetSize)
        {
            int nwidth = targetSize, nHeight = targetSize;
            if (src.PixelWidth > src.PixelHeight)
            {
                nwidth = targetSize;
                nHeight = (int)((float)src.PixelHeight * ((float)targetSize / (float)src.PixelWidth));
            }
            else
            {
                nHeight = targetSize;
                nwidth = (int)((float)src.PixelWidth * ((float)targetSize / (float)src.PixelHeight));
            }

            CommonTools.Log("Resize image [" + src.PixelWidth + "x" + src.PixelHeight + "] ==> [" + nwidth + "x" + nHeight + "]");

            var rect = new Rect(0, 0, nwidth, nHeight);
            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(src, rect));
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            var resizedImage = new RenderTargetBitmap(nwidth, nHeight, 96, 96, PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return resizedImage;
        }

#endregion


#region Web ==================

        /*
        public static string TryGetURL(string str)
        {
            if (str == null)
            {
                return null;
            }

            int index = -1;
            if ((index = str.IndexOf("http://")) >= 0)
            {

            }
            else if ((index = str.IndexOf("https://")) >= 0)
            {

            }
            else
            {

            }

            if (index >= 0)
            {
                return str.Substring(index);
            }
            else
            {
                return null;
            }
        }

        public static string URLEncode(string str)
        {
            return WebUtility.UrlEncode(str);
        }

        public static string URLDecode(string str)
        {
            return WebUtility.UrlDecode(str);
        }

        public static string ToJson<T>(object obj)
        {
            try
            {
                JavaScriptSerializer json = new JavaScriptSerializer();
                string strJson = json.Serialize(obj);
                return strJson;
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return null;
            }
        }

        public static T ParseJason<T>(string jason)
        {
            string ret = string.Empty;

            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                return ser.Deserialize<T>(jason);
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return default(T);
            }
        }        
        */

#endregion

#region Conversion ==================

        public static byte[] SHA256Hash(byte[] src)
        {
            return SHA256.Create().ComputeHash(src);
        }

        public static string TryGetURL(string str)
        {
            if (str == null)
            {
                return null;
            }

            int index = -1;
            if ((index = str.IndexOf("http://")) >= 0)
            {

            }
            else if ((index = str.IndexOf("https://")) >= 0)
            {

            }
            else
            {

            }

            if (index >= 0)
            {
                return str.Substring(index);
            }
            else
            {
                return null;
            }
        }

#if !DOTNET_4
        public static string URLEncode(string str)
        {
            return WebUtility.UrlEncode(str);
        }

        public static string URLDecode(string str)
        {
            return WebUtility.UrlDecode(str);
        }

        public static string ToJson<T>(object obj)
        {
            try
            {
                JavaScriptSerializer json = new JavaScriptSerializer();
                string strJson = json.Serialize(obj);
                return strJson;
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return null;
            }
        }

        public static T ParseJason<T>(string jason)
        {
            string ret = string.Empty;

            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                return ser.Deserialize<T>(jason);
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return default(T);
            }
        }
#endif

        public static T DeepCopy<T>(object objectToCopy)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

#endregion
    }
}
