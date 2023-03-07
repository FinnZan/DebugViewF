using FinnZan.Utilities;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace FileHeaderChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string _root = @"E:\Dell\Projects\AWCC\src\main_repo\performance\daan";
        private string _root = @"E:\Dell\Projects\AWCC\src\main_repo\performance\contract";             

        private HeaderChecker _fileHeaderChecker;

        public MainWindow()
        {
            CommonTools.InitializeDebugger("DELL");
            CommonTools.Log("=======================");

            InitializeComponent();

            _fileHeaderChecker = new HeaderChecker(_root);
        }

        #region UI event

        private void btnScanAll_Click(object sender, RoutedEventArgs e)
        {
            _fileHeaderChecker.ScanAll();

            _missingFiles.ItemsSource = null;
            _missingFiles.ItemsSource = _fileHeaderChecker.MissingFiles;
        }


        private void btnAddAll_Click(object sender, RoutedEventArgs e)
        {
            _fileHeaderChecker.AddAll();
        }

        private void _missingFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selected = (sender as Label).DataContext as CodeFile;
                Process.Start(@"C:\Program Files\Notepad++\notepad++.exe", selected.FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button).DataContext as CodeFile;
            _fileHeaderChecker.AddOrUpdateHeader(selected);
        }

        #endregion
    }
}
