using System;
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
using System.Xml.Linq;
using Path = System.IO.Path;

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
            
            tbSolutionPath.Text = @"E:\Dell\Projects\DPM\src\dpm\Source";

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

        private List<AssemblyFile> Load()
        {
            var allProjects = Directory.GetFiles(_path, "*.csproj", SearchOption.AllDirectories);

            var results = new List<AssemblyFile>();

            foreach(var f in allProjects)
            {
                var proj = XDocument.Load(f).Root;
                var projName = System.IO.Path.GetFileName(f);

                foreach(var ig in proj.Elements().Where(e => e.Name.LocalName == "ItemGroup"))
                {
                    foreach(var r in ig.Elements().Where(e => e.Name.LocalName == "Reference"))
                    {
                        try
                        {
                            var hint = GetHintPath(r);

                            if(hint != null)
                            {
                                var fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(f), hint));

                                AssemblyFile asmb = results.SingleOrDefault(o => o.FullPath == fullPath);
                                if (asmb == null)
                                {
                                    var time = GetFileTime(fullPath);
                                    asmb = new AssemblyFile(fullPath, time);
                                    results.Add(asmb);
                                }
                                asmb.Projects.Add(projName);
                            }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
            }

            results.Sort(
                delegate(AssemblyFile a, AssemblyFile b)
                {
                    if (a.Name != b.Name)
                    {
                        return a.Name.CompareTo(b.Name);
                    }
                    else
                    {
                        return a.FullPath.CompareTo(b.FullPath);
                    }
                }
            );

            return results;
        }

        private string GetHintPath(XElement r)
        {
            if (r.Elements().Any(e => e.Name.LocalName == "HintPath"))
            {
                return r.Elements().First(e => e.Name.LocalName == "HintPath").Value;
            }
            else
            {
                return null;
            }
        }
        private DateTime GetFileTime(string fullPath)
        {
            var lastWrite = File.GetLastWriteTime(fullPath);
            var create = File.GetCreationTime(fullPath);

            return lastWrite < create ? create : lastWrite;
        }
    }
}
