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
