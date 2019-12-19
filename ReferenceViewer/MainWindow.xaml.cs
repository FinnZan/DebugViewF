using System;
using System.Diagnostics;
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

        private const string TextEditor = @"C:\Program Files (x86)\Notepad++\notepad++.exe";

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

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selected = ((ListBox)sender).SelectedItem;

                switch(selected)
                {
                    case NugetReference reference:
                    {
                        var proj = reference;
                        Process.Start(TextEditor, proj.ProjecFile);
                        break;
                    }
                    case AssemblyReference reference:
                    {
                        var proj = reference;
                        Process.Start(TextEditor, proj.ProjecFile);
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}