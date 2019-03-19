﻿namespace FinnZan.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Media.Imaging;

    public partial class CommonTools
    {
        private static string mUUID = null;

        public static int UUID_SIZE
        {
            get
            {
                return mUUID.Length;
            }
        }

        public static string GetUUID()
        {
            if (mUUID == null)
            {
                mUUID = string.Format("000000000{0}", SystemInfoReader.GeServiceTag());
            }
            return mUUID;
        }

        public static string GetProgramVersion()
        {
            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
            catch (Exception ex)
            {
                return "N/A";
            }
        }

        public static string GetUserName()
        {
            return Environment.UserName;
        }
        
        public static String GetMachineName()
        {
            return Environment.MachineName;
        }

        public static string GetDisplayName()
        {
            try
            {
                return System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName;
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return null;
            }
        }

        public static string GetOSFriendlyName()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            return result;
        }

        public static string GetExecutingAssemblyPath()
        {
            string path = null;

            try
            {
                path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            }
            catch
            {
            }

            return path;
        }

        public static string GetExecutingAssemblyDirectoryPath()
        {
            string path = null;

            try
            {
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            catch
            {
            }

            return path;
        }

        public static bool OpenURL(string url)
        {
            bool bSuccess = false;
            try
            {
                if (false == String.IsNullOrEmpty(url))
                {
                    CommonTools.Log("[Opening URL =>" + @url + "].");
                    System.Diagnostics.Process.Start(url);
                    bSuccess = true;
                }
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
            }
            return bSuccess;
        }

        public static bool LogOnUser(string user, string password)
        {
            try
            {
                int ret;
                if (LogonUtility.IsValidCredentialByLogonUser(user, password, out ret))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return false;
            }
        }

        public static bool OpenInDefaultApplication(string path, string arg = null)
        {
            bool bSuccess = false;
            try
            {
                if (File.Exists(path) && !IsUnsafeFileType(path))
                {
                    CommonTools.Log("[Opening " + @path + "].");
                    System.Diagnostics.Process.Start(path, arg);
                    bSuccess = true;
                    CommonTools.Log("[" + path + "] opened.");
                }
                else
                {
                    CommonTools.Log("[" + path + "] not found.");
                }
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
            }

            return bSuccess;
        }

        public static BitmapImage GetProfilePicture()
        {
            var sb = new StringBuilder(1000);
            Win32Native.GetUserTilePath(Environment.UserName, 0x80000000, sb, sb.Capacity);
            string path = sb.ToString();
            CommonTools.Log("UserTilePath [" + Environment.UserName + "] [" + path + "]");
            BitmapImage bmp = new BitmapImage(new Uri(path));
            bmp.Freeze();
            return bmp;
        }

        public static System.Windows.Point DPIScale
        {
            get
            {
                var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
                var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

                var dpiX = (int)dpiXProperty.GetValue(null, null);
                var dpiY = (int)dpiYProperty.GetValue(null, null);

                System.Windows.Point p = new System.Windows.Point((double)dpiX / 96F, (double)dpiY / 96F);

                return p;
            }
        }

        public static bool IsMouseLeftButtonDown()
        {
            return Control.MouseButtons == MouseButtons.Left;
        }

        public static System.Drawing.Point GetMousePosition()
        {
            System.Drawing.Point point = Control.MousePosition;
            return new System.Drawing.Point((int)((double)point.X / DPIScale.X), (int)((double)point.Y / DPIScale.Y));
        }
    }
}
