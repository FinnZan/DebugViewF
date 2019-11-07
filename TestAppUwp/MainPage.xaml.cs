using FinnZan.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestAppUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            CommonTools.InitializeDebugger("LogTest");
            CommonTools.Log("Start.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string a = null;

            try
            {
                a.ToArray();
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
            }

            try
            {
                CommonTools.Log(DateTime.Now.ToShortTimeString());
                Trace.WriteLine("click");
                var t = Task.Run(() =>
                {
                    for (var i = 0; i < 10; i++)
                    {
                        CommonTools.Log($"{DateTime.Now}");
                    }
                });

                t.Wait();
                CommonTools.Log("done.");

            }
            catch (Exception ex)
            {
                lbOut.Text = ex.ToString();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
