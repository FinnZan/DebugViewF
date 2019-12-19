using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private string _root = "";


        public MainWindow()
        {
            InitializeComponent();
            
            tbSolutionPath.Text = @"E:\Dell\Projects\DPM\src\dpm\Source";
            Load();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F5)
            {
                Load();
            }
        }

        private void Load()
        {
            _root = Path.Combine(tbSolutionPath.Text);

            if (!Directory.Exists(_root))
            {
                MessageBox.Show("Folder not exist");
            }

            var allProjects = Directory.GetFiles(_root, "*.csproj", SearchOption.AllDirectories);

            var assemblies = new List<AssemblyFile>();
            var packages = new List<NugetPackage>();

            foreach (var projFile in allProjects)
            {
                var projNode = XDocument.Load(projFile).Root;
                var projName = System.IO.Path.GetFileName(projFile);

                foreach(var ig in projNode.Elements().Where(e => e.Name.LocalName == "ItemGroup"))
                {
                    foreach(var r in ig.Elements().Where(e => e.Name.LocalName == "Reference"))
                    {
                        AddReference(r, projName, projFile, assemblies);
                    }

                    foreach (var r in ig.Elements().Where(e => e.Name.LocalName == "Content" && e.Attribute("Include") != null))
                    {
                        AddLink(r, projName, projFile, assemblies);
                    }

                    foreach (var r in ig.Elements().Where(e => e.Name.LocalName == "PackageReference"))
                    {
                        AddPackageReference(r, projName, projFile, packages);
                    }
                }
            }

            assemblies.Sort((a, b) => a.IsLocal != b.IsLocal ? (a.IsLocal ? 1 : -1) : (a.Name != b.Name ? a.Name.CompareTo(b.Name) : a.ActualPath.CompareTo(b.ActualPath)));


            packages.Sort((a, b) => a.HasConflict != b.HasConflict ? (a.HasConflict ? -1 : 1) : a.Name.CompareTo(b.Name));

            lbResult.ItemsSource = null;
            lbResult.ItemsSource = assemblies;

            lbxNugetResult.ItemsSource = null;
            lbxNugetResult.ItemsSource = packages;
        }

        private void AddPackageReference(XElement r, string projName, string projectFile, List<NugetPackage> packages)
        {
            try
            {
                var name = r.Attribute("Include").Value;

                if (name != null)
                {
                    NugetPackage pkg = packages.SingleOrDefault(o => o.Name == name);

                    var version = "Unknown";

                    if (r.Elements().Any(e => e.Name.LocalName == "Version"))
                    {
                        version = r.Elements().First(e => e.Name.LocalName == "Version").Value;
                    }
                    
                    if (pkg == null)
                    {
                        pkg = new NugetPackage(name);
                        packages.Add(pkg);
                    }
                    pkg.Projects.Add(new NugetReference(projName, projectFile, version));

                    pkg.Projects.Sort((a, b) => b.Version.CompareTo(a.Version));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void AddReference(XElement r, string projName, string projectFile, List<AssemblyFile> results)
        {
            try
            {
                var hint = GetHintPath(r);

                if(hint != null)
                {
                    var fullPath = ResolveFullPath(projectFile, hint);

                    AssemblyFile asmb = results.SingleOrDefault(o => o.ActualPath == fullPath);
                    if(asmb == null)
                    {
                        var time = GetFileTime(fullPath);
                        asmb = new AssemblyFile(fullPath, time, fullPath.StartsWith(_root));
                        results.Add(asmb);
                    }
                    asmb.Projects.Add(new AssemblyReference(projName, projectFile, UsageType.Reference, hint));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AddLink(XElement r, string projName, string projectFile, List<AssemblyFile> results)
        {
            try
            {
                var link = r.Attribute("Include").Value;
                if(link.EndsWith(".dll"))
                {
                    var fullPath = ResolveFullPath(projectFile, link);

                    AssemblyFile asmb = results.SingleOrDefault(o => o.ActualPath == fullPath);
                    if(asmb == null)
                    {
                        var time = GetFileTime(fullPath);
                        asmb = new AssemblyFile(fullPath, time, fullPath.StartsWith(_root));
                        results.Add(asmb);
                    }
                    asmb.Projects.Add(new AssemblyReference(projName, projectFile, UsageType.Link, link));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private string ResolveFullPath(string projectFile, string path)
        {
            var fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectFile), path));
            fullPath = fullPath.Replace("$(Configuration)", "Debug");
            return fullPath;
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

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selected = ((ListBox)sender).SelectedItem;

                if (selected is NugetReference)
                {
                    var proj = selected as NugetReference;

                    var cmd = $@"'C:\Program Files (x86)\Notepad++\notepad++.exe'' ''{proj.ProjecFile}''";
                    Process.Start(@"C:\Program Files (x86)\Notepad++\notepad++.exe", proj.ProjecFile);
                }
                else if(selected is AssemblyReference)
                {
                    var proj = selected as AssemblyReference;
                    var cmd = $@"'C:\Program Files (x86)\Notepad++\notepad++.exe'' ''{proj.ProjecFile}''";
                    Process.Start(@"C:\Program Files (x86)\Notepad++\notepad++.exe", proj.ProjecFile);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
