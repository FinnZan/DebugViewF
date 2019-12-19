using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ReferenceViewer
{
    public class ReferenceFinder
    {
        private string _root = "";

        public List<AssemblyFile> Assemblies { get; } = new List<AssemblyFile>();
        public List<NugetPackage> NuGetPackages { get; } = new List<NugetPackage>();

        public ReferenceFinder()
        {
        }

        public void Load(string root)
        {
            _root = Path.Combine(root);

            if(!Directory.Exists(_root))
            {
                throw new Exception("Folder not exist");
            }

            Assemblies.Clear();
            NuGetPackages.Clear();

            var allProjects = Directory.GetFiles(_root, "*.csproj", SearchOption.AllDirectories);

            foreach(var projFile in allProjects)
            {
                var projNode = XDocument.Load(projFile).Root;
                var projName = System.IO.Path.GetFileName(projFile);

                foreach(var ig in projNode.Elements().Where(e => e.Name.LocalName == "ItemGroup"))
                {
                    foreach(var r in ig.Elements().Where(e => e.Name.LocalName == "Reference"))
                    {
                        AddReference(r, projName, projFile);
                    }

                    foreach(var r in ig.Elements().Where(e => e.Name.LocalName == "Content"))
                    {
                        AddLink(r, projName, projFile);
                    }

                    foreach(var r in ig.Elements().Where(e => e.Name.LocalName == "PackageReference"))
                    {
                        AddPackageReference(r, projName, projFile);
                    }
                }
            }

            Assemblies.Sort((a, b) => a.IsLocal != b.IsLocal ? (a.IsLocal ? 1 : -1) : (a.Name != b.Name ? a.Name.CompareTo(b.Name) : a.ActualPath.CompareTo(b.ActualPath)));

            NuGetPackages.Sort((a, b) => a.IsConsistent != b.IsConsistent ? (a.IsConsistent ? 1 : -1) : a.Name.CompareTo(b.Name));
        }

        private void AddPackageReference(XElement r, string projName, string projectFile)
        {
            var name = r.Attribute("Include")?.Value;

            NugetPackage pkg = NuGetPackages.SingleOrDefault(o => o.Name == name);

            var version = "Unknown";

            if(r.Elements().Any(e => e.Name.LocalName == "Version"))
            {
                version = r.Elements().First(e => e.Name.LocalName == "Version").Value;
            }

            if(pkg == null)
            {
                pkg = new NugetPackage(name);
                NuGetPackages.Add(pkg);
            }
            pkg.Projects.Add(new NugetReference(projName, projectFile, version));

            pkg.Projects.Sort((a, b) => b.Version.CompareTo(a.Version));
        }

        private void AddReference(XElement r, string projName, string projectFile)
        {
            var hint = GetHintPath(r);

            if(hint == null)
            {
                return;
            }

            var fullPath = ResolveFullPath(projectFile, hint);

            AssemblyFile asmb = Assemblies.SingleOrDefault(o => o.ActualPath == fullPath);
            if(asmb == null)
            {
                var time = GetFileTime(fullPath);
                asmb = new AssemblyFile(fullPath, time, fullPath.StartsWith(_root));
                Assemblies.Add(asmb);
            }
            asmb.Projects.Add(new AssemblyReference(projName, projectFile, UsageType.Reference, hint));
        }

        private void AddLink(XElement r, string projName, string projectFile)
        {
            var link = r.Attribute("Include")?.Value;

            if (link == null)
            {
                return;
            }

            if (link.EndsWith(".dll"))
            {
                var fullPath = ResolveFullPath(projectFile, link);

                AssemblyFile asmb = Assemblies.SingleOrDefault(o => o.ActualPath == fullPath);
                if(asmb == null)
                {
                    var time = GetFileTime(fullPath);
                    asmb = new AssemblyFile(fullPath, time, fullPath.StartsWith(_root));
                    Assemblies.Add(asmb);
                }
                asmb.Projects.Add(new AssemblyReference(projName, projectFile, UsageType.Link, link));
            }
        }

        #region Small things

        private string ResolveFullPath(string projectFile, string path)
        {
            var fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectFile), path));
            fullPath = fullPath.Replace("$(Configuration)", "Debug");
            return fullPath;
        }

        private string GetHintPath(XElement r)
        {
            if(r.Elements().Any(e => e.Name.LocalName == "HintPath"))
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

        #endregion
    }
}