﻿using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Linq;

namespace ReferenceViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _path = @"E:\Dell\Projects\DPM\src\dpm\Source";

        public MainWindow()
        {
            InitializeComponent();

            lbResult.ItemsSource = Load();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F5)
            {
                lbResult.ItemsSource = null;
                lbResult.ItemsSource = Load();
            }
        }

        private List<KeyValuePair<string, List<string>>> Load()
        {
            var allProjects = Directory.GetFiles(_path, "*.csproj", SearchOption.AllDirectories);

            var results = new Dictionary<string, List<string>>();

            foreach (var f in allProjects)
            {
                var proj = XDocument.Load(f).Root;
                var projName = System.IO.Path.GetFileName(f);

                foreach (var ig in proj.Elements().Where(e => e.Name.LocalName == "ItemGroup"))
                {
                    foreach (var r in ig.Elements().Where(e => e.Name.LocalName == "Reference"))
                    {
                        var hint = GetHintPath(r);

                        if (hint != null)
                        {
                            if (results.ContainsKey(hint))
                            {
                                results[hint].Add(projName);
                            }
                            else
                            {
                                results.Add(hint, new List<string>() { projName });
                            }
                        }
                    }
                }
            }

            List<KeyValuePair<string, List<string>>> ret = results.ToList();

            ret.Sort(delegate (KeyValuePair<string, List<string>> pair1, KeyValuePair<string, List<string>> pair2)
                {
                    return pair1.Key.CompareTo(pair2.Key);
                }
            );

            return ret;
        }

        private string GetHintPath(XElement r)
        {
            if (r.Elements().Where(e => e.Name.LocalName == "HintPath").Any())
            {
                return r.Elements().Where(e => e.Name.LocalName == "HintPath").First().Value;
            }
            else
            {
                return null;
            }
        }
    }
}