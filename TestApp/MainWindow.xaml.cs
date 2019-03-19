using FinnZan.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CommonTools.InitializeDebugger("LogTest", App.Current);

            Trace.WriteLine("here.");            
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
                lbOut.Content = ex.ToString();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var app = App.Current.MainWindow;            
        }
    }
}
