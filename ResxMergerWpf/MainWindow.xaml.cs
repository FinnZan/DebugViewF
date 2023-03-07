using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace ResxMergerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, string>  _oldBatch = new Dictionary<string, string>();
        private Dictionary<string, string> _newBatch = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();

            ProcessResource();
        }

        private void ProcessResource()
        {
            var resxFile = @"E:\Temp\RESX\en.resx";
            var resxFileAddition = @"E:\Temp\RESX\addition.resx";


            using (var writer = new ResXResourceWriter(resxFile))
            {
                foreach (var entry in _oldBatch)
                {
                    writer.AddResource(entry.Key, entry.Value);
                }
            }
        }

        private void SoruceBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                tbxSource.Text = openFileDialog.FileName;
                _oldBatch.Clear();

                using (var reader = new ResXResourceReader(openFileDialog.FileName))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        _oldBatch.Add(entry.Key.ToString(), entry.Value.ToString());                    
                    }
                    lbSource.Content = $"{_oldBatch.Count} strings.";
                }
            }
        }

        private void AdditionBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                tbxAddition.Text = openFileDialog.FileName;
                _newBatch.Clear();

                using (var addition = new ResXResourceReader(openFileDialog.FileName))
                {
                    foreach (DictionaryEntry entry in addition)
                    {
                        _newBatch.Add(entry.Key.ToString(), entry.Value.ToString());
                    }
                    lbAddition.Content = $"{_newBatch.Count} strings.";
                }
            }
        }

        private void Merge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var count = 0;
                foreach (var p in _newBatch)
                {
                    if (_oldBatch.ContainsKey(p.Key))
                    {
                        count++;
                    }
                    _oldBatch[p.Key] = p.Value;
                }

                using (var writer = new ResXResourceWriter(tbxSource.Text))
                {
                    foreach (var entry in _oldBatch)
                    {
                        writer.AddResource(entry.Key, entry.Value);
                    }
                }

                tbOutput.Text = $"{count} changed. {_newBatch.Count - count} Added.";
            }
            catch (Exception ex) 
            {
                tbOutput.Text = ex.ToString();
            }
        }
    }
}
