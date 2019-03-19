namespace FinnZan.Utilities
{
    using System;

    public class SystemInfoReader
    {
        private const int SERVICE_TAG_SIZE = 7;

        public static string GeServiceTag() 
        {
            try
            {
                string ret = Win32_BIOSReader.Read().SerialNumber;
                
                if (ret != null)
                {
                    if (ret.Length > SERVICE_TAG_SIZE)
                    {
                        ret = ret.Substring(0, SERVICE_TAG_SIZE);
                    }
                    else 
                    {
                        string strDefault = "1234567";
                        ret = ret + strDefault.Substring(0, SERVICE_TAG_SIZE - ret.Length);
                    }
                    return ret;
                }
                else
                {
                    return "0000000";
                }
            }
            catch (Exception ex) 
            {
                CommonTools.HandleException(ex);
                return "1234567";
            }
        }

        public static string GetManufacturer() 
        {
            Win32_BIOS bios = Win32_BIOSReader.Read();
            if (bios != null && bios.Manufacturer != null )
            {
                return bios.Manufacturer;
            }
            else 
            {
                return "Unknwon";
            }
        }
    }
}
