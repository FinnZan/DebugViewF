﻿using System;
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

namespace ReferenceViewer
{
    /// <summary>
    /// Interaction logic for AssemblyView.xaml
    /// </summary>
    public partial class AssemblyView : UserControl
    {
        public AssemblyView()
        {
            InitializeComponent();
        }
        
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selected = ((ListBox)sender).SelectedItem as AssemblyReference;
                Process.Start(App.TextEditor, selected.ProjecFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selected = this.DataContext as AssemblyFile;
                var folder = System.IO.Path.GetDirectoryName(selected.ActualPath);         
                Process.Start(folder);           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
