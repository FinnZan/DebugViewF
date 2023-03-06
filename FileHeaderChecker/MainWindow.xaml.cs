using FinnZan.Utilities;
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

namespace FileHeaderChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string _root = @"E:\Dell\Projects\AWCC\src\main_repo\performance\daan";
        private string _root = @"E:\Dell\Projects\AWCC\src\main_repo\performance\contract";
   
        private string[] _ignoredFolders = { "obj", "AwccWinUI", "AwccWpfCore", "AgentService", "UnitTests", "ServiceSimulation" };

        private List<string> _workingSet = new List<string>();

        public MainWindow()
        {
            CommonTools.InitializeDebugger("DELL");

            InitializeComponent();
        }

        public void ScanAll() 
        {
            var files = Directory.GetFiles(_root, "*.cs", SearchOption.AllDirectories).Where(x => !IsIgnored(x));

            _workingSet = new List<string>();
            
            foreach (var f in files)
            {
                try
                {
                    var lines = File.ReadAllLines(f);
                    if (!AlreadyHasHeader(lines))
                    {
                        _workingSet.Add(f);
                    }
                }
                catch (Exception ex) 
                {
                    CommonTools.HandleException(ex);
                }
            }

            _workingSet.Sort();

            _missingFiles.ItemsSource = null;
            _missingFiles.ItemsSource= _workingSet;

            this.Title = $"{_workingSet.Count}/{files.Count()} headers missed.";
        }

        public void AddAll()
        {   
            foreach (var f in _workingSet)
            {
                try
                {                    
                    AddHeader(f);
                }
                catch (Exception ex)
                {
                    CommonTools.HandleException(ex);
                }
            }
        }

        private bool AlreadyHasHeader(string[] lines) 
        {
            return !lines[0].StartsWith("using");
        }

        private bool IsIgnored(string path) 
        {
            if (path.EndsWith(".g.cs") || path.EndsWith(".Designer.cs")) 
            {
                return true;
            }

            try
            {
                var folder = System.IO.Path.GetDirectoryName(path);
                foreach (var f in _ignoredFolders)
                {
                    if (folder.Contains(@$"\{f}"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex) 
            {
                CommonTools.HandleException(ex);
            }

            return false;
        }

        private void AddHeader(string fullPath) 
        {
            try
            {  
                var fn = System.IO.Path.GetFileName(fullPath);

                //var header = $"// <copyright file=\"{fn}\" company=\"Dell Inc.\">\n" +
                //         "//     Copyright © 1999 - 2022 Dell Inc. All rights reserved\n" +
                //         "// </copyright>";

                var header = 
@"//
// DELL PROPRIETARY INFORMATION
//
// This software contains the intellectual property of Dell Inc. 
// Use of this software and the intellectual property contained
// therein is expressly limited to the terms and conditions of
// the License Agreement under which it is provided by or
// on behalf of Dell Inc. or its subsidiaries.
//
// Copyright 2022 Dell Inc. or its subsidiaries. All Rights Reserved.
//
// DELL INC. MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE SOFTWARE, 
// EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.  
// DELL SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF USING, 
// MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS DERIVATIVES.
//
";

                var lines = new List<string>(File.ReadAllLines(fullPath));
                lines.Insert(0, header);
                File.WriteAllLines(fullPath, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #region UI event

        private void btnScanAll_Click(object sender, RoutedEventArgs e)
        {
            ScanAll();
        }


        private void btnAddAll_Click(object sender, RoutedEventArgs e)
        {
            AddAll();
        }

        private void _missingFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selected = (sender as Label).DataContext as string;
                Process.Start(@"C:\Program Files\Notepad++\notepad++.exe", selected);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button).DataContext as string;
            AddHeader(selected);
        }

        #endregion
    }
}
