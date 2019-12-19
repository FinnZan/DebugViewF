using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ReferenceViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReferenceFinder _referenceFinder = new ReferenceFinder();
        
        public MainWindow()
        {
            InitializeComponent();

            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.KeyUpEvent, new System.Windows.Input.KeyEventHandler(TextBox_KeyUp));
            
            tbSolutionPath.Text = @"E:\Dell\Projects\DPM\src\dpm\Source";

            Reload();
        }

        private void Reload()
        {
            try
            {
                _referenceFinder.Load(tbSolutionPath.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            lbResult.ItemsSource = null;
            lbResult.ItemsSource = _referenceFinder.Assemblies;

            lbxNugetResult.ItemsSource = null;
            lbxNugetResult.ItemsSource = _referenceFinder.NuGetPackages;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(sender == tbSolutionPath && e.Key == Key.Enter)
            {
                Reload();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F5)
            {
                Reload();
            }
        }
    }

    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;

            return b ? Color.FromRgb(0xEE, 0xEE, 0xEE) : Color.FromRgb(0xFF, 0xEE, 0xEE);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}