using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            var path = Properties.Settings.Default["Path"] as string;
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path)) 
            {
                tbSolutionPath.Text = path;
                Reload();
            }
        }

        private void Reload()
        {
            try
            {
                _referenceFinder.Load(tbSolutionPath.Text);
                Properties.Settings.Default["Path"] = tbSolutionPath.Text;
                Properties.Settings.Default.Save();
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

            lbxProjectResult.ItemsSource = null;
            lbxProjectResult.ItemsSource = _referenceFinder.Projects;
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
}