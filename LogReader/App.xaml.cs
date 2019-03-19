using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FinnZan.Utilities
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool RunAsServer
        {
            get; set;
        }

        public static string AppName
        {
            get;
            set;
        }

        Window _win;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                _win = new MainWindow();

                if (e.Args.Length > 0)
                {
                    RunAsServer = e.Args[0] == "RunAsServer";

                    if (RunAsServer)
                    {
                        if (e.Args.Length > 1)
                        {
                            AppName = e.Args[1];
                        }
                        else
                        {                            
                            var dialog = new AppNameDialog();
                            if (dialog.ShowDialog() == true)
                            {
                                AppName = dialog.AppName;
                            }
                            else
                            {
                                Environment.Exit(0);
                            }                            
                        }
                    }
                }
                
                _win.Show();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }            
        }
    }
}
