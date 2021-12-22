using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ReferenceViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string TextEditor {
            get 
            {
                const string v86  = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
                const string v64 = @"C:\Program Files\Notepad++\notepad++.exe";

                if (File.Exists(v86))
                {
                    return v86;
                }
                else
                {
                    return v64;
                }
            }
        }
    }
}
