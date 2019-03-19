namespace FinnZan.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Management;
    internal class Win32_BIOS
    {
        public ushort[] BiosCharacteristics;
        public string[] BIOSVersion;
        public string BuildNumber;
        public string Caption;
        public string CodeSet;
        public string CurrentLanguage;
        public string Description;
        public string IdentificationCode;
        public ushort? InstallableLanguages;
        public DateTime? InstallDate;
        public string LanguageEdition;
        public string[] ListOfLanguages;
        public string Manufacturer;
        public string Name;
        public string OtherTargetOS;
        public bool? PrimaryBIOS;
        public string ReleaseDate;
        public string SerialNumber;
        public string SMBIOSBIOSVersion;
        public ushort? SMBIOSMajorVersion;
        public ushort? SMBIOSMinorVersion;
        public bool? SMBIOSPresent;
        public string SoftwareElementID;
        public ushort? SoftwareElementState;
        public string Status;
        public ushort? TargetOperatingSystem;
        public string Version;
    }

    internal class Win32_BIOSReader
    {
        public static Win32_BIOS Read()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            ManagementObjectCollection collection = searcher.Get();

            var items = new List<Win32_BIOS>();
            foreach (ManagementObject obj in collection)
            {
                var item = new Win32_BIOS();
                item.BiosCharacteristics = (ushort[])obj["BiosCharacteristics"];
                item.BIOSVersion = (string[])obj["BIOSVersion"];
                item.BuildNumber = (string)obj["BuildNumber"];
                item.Caption = (string)obj["Caption"];
                item.CodeSet = (string)obj["CodeSet"];
                item.CurrentLanguage = (string)obj["CurrentLanguage"];
                item.Description = (string)obj["Description"];
                item.IdentificationCode = (string)obj["IdentificationCode"];
                item.InstallableLanguages = (ushort?)obj["InstallableLanguages"];
                item.InstallDate = (DateTime?)obj["InstallDate"];
                item.LanguageEdition = (string)obj["LanguageEdition"];
                item.ListOfLanguages = (string[])obj["ListOfLanguages"];
                item.Manufacturer = (string)obj["Manufacturer"];
                item.Name = (string)obj["Name"];
                item.OtherTargetOS = (string)obj["OtherTargetOS"];
                item.PrimaryBIOS = (bool?)obj["PrimaryBIOS"];
                item.ReleaseDate = (string)obj["ReleaseDate"];
                item.SerialNumber = (string)obj["SerialNumber"];
                item.SMBIOSBIOSVersion = (string)obj["SMBIOSBIOSVersion"];
                item.SMBIOSMajorVersion = (ushort?)obj["SMBIOSMajorVersion"];
                item.SMBIOSMinorVersion = (ushort?)obj["SMBIOSMinorVersion"];
                item.SMBIOSPresent = (bool?)obj["SMBIOSPresent"];
                item.SoftwareElementID = (string)obj["SoftwareElementID"];
                item.SoftwareElementState = (ushort?)obj["SoftwareElementState"];
                item.Status = (string)obj["Status"];
                item.TargetOperatingSystem = (ushort?)obj["TargetOperatingSystem"];
                item.Version = (string)obj["Version"];

                items.Add(item);
            }

            if (items.Count > 0)
            {
                return items[0];
            }
            else 
            {
                return null;
            }
        }
    }
}
