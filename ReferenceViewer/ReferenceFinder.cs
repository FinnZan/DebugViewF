﻿using System;
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

        public List<Project> Projects { get; } = new List<Project>();

        public ReferenceFinder()
        {
        }

        public void Load(string root)
        {
            _root = Path.Combine(root);

            if (!Directory.Exists(_root))
            {
                throw new Exception("Folder not exist");
            }

            Assemblies.Clear();
            NuGetPackages.Clear();
            Projects.Clear();

            var allProjects = Directory.GetFiles(_root, "*.csproj", SearchOption.AllDirectories);

            foreach (var projFile in allProjects)
            {
                var projNode = XDocument.Load(projFile).Root;
                var projName = System.IO.Path.GetFileName(projFile);

                //if (projName.EndsWith("f.csproj"))
                //{
                //    continue;
                //}

                ProjectType projectType = ProjectType.Framework;

                foreach (var pg in projNode.Elements().Where(e => e.Name.LocalName == "PropertyGroup"))
                {
                    foreach (var r in pg.Elements().Where(e => e.Name.LocalName == "TargetFramework"))
                    {
                        if (r.Value.StartsWith("netstandard"))
                        {
                            projectType = ProjectType.Standard;
                        }
                        else 
                        {
                            projectType = ProjectType.Core;
                        }
                    }
                }

                foreach (var ig in projNode.Elements().Where(e => e.Name.LocalName == "ItemGroup"))
                {
                    foreach (var r in ig.Elements().Where(e => e.Name.LocalName == "Reference"))
                    {
                        AddReference(r, projName, projFile, projectType);
                    }

                    foreach (var r in ig.Elements().Where(e => e.Name.LocalName == "Content"))
                    {
                        AddLink(r, projName, projFile, projectType);
                    }

                    foreach (var r in ig.Elements().Where(e => e.Name.LocalName == "PackageReference"))
                    {
                        AddPackageReference(r, projName, projFile, projectType);
                    }

                    foreach (var r in ig.Elements().Where(e => e.Name.LocalName == "ProjectReference"))
                    {
                        AddProjectReference(r, projName, projFile, projectType);
                    }
                }
            }

            Assemblies.Sort((a, b) => a.IsLocal != b.IsLocal ? (a.IsLocal ? 1 : -1) : (a.Name != b.Name ? a.Name.CompareTo(b.Name) : a.ActualPath.CompareTo(b.ActualPath)));

            NuGetPackages.Sort((a, b) => a.IsConsistent != b.IsConsistent ? (a.IsConsistent ? 1 : -1) : a.Name.CompareTo(b.Name));

            Projects.Sort((a, b) => a.Name.CompareTo(b.Name));
        }

        private void AddPackageReference(XElement r, string projName, string projectFile, ProjectType projectType)
        {
            var name = r.Attribute("Include")?.Value;

            NugetPackage pkg = NuGetPackages.SingleOrDefault(o => o.Name == name);

            var version = r.Attribute("Version")?.Value;

            if (r.Elements().Any(e => e.Name.LocalName == "Version"))
            {
                version = r.Elements().First(e => e.Name.LocalName == "Version").Value;
            }

            if (pkg == null)
            {
                pkg = new NugetPackage(name);
                NuGetPackages.Add(pkg);
            }

            pkg.Projects.Add(new NugetReference(projName, projectFile, version, projectType));

            pkg.Projects.Sort((a, b) => b.Version.CompareTo(a.Version));
        }

        private void AddReference(XElement r, string projName, string projectFile, ProjectType projectType)
        {
            var hint = GetHintPath(r);

            if (hint == null)
            {
                return;
            }

            var fullPath = ResolveFullPath(projectFile, hint);

            AssemblyFile asmb = Assemblies.SingleOrDefault(o => o.ActualPath == fullPath);
            if (asmb == null)
            {
                var time = GetFileTime(fullPath);
                asmb = new AssemblyFile(fullPath, time, File.Exists(fullPath) && fullPath.StartsWith(_root));
                Assemblies.Add(asmb);
            }
            asmb.Projects.Add(new AssemblyReference(projName, projectFile, UsageType.Reference, hint, projectType));
        }

        private void AddLink(XElement r, string projName, string projectFile, ProjectType projectType)
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
                if (asmb == null)
                {
                    var time = GetFileTime(fullPath);
                    asmb = new AssemblyFile(fullPath, time, fullPath.StartsWith(_root));
                    Assemblies.Add(asmb);
                }
                asmb.Projects.Add(new AssemblyReference(projName, projectFile, UsageType.Link, link, projectType));
            }
        }

        private void AddProjectReference(XElement r, string projName, string projectFile, ProjectType projectType)
        {
            var link = r.Attribute("Include")?.Value;

            var fullPath = ResolveFullPath(projectFile, link);

            var proj = Projects.SingleOrDefault(o => o.ActualPath == fullPath);

            if (proj == null)
            {
                var time = GetFileTime(fullPath);
                proj = new Project(fullPath);
                Projects.Add(proj);
            }

            proj.Projects.Add(new AssemblyReference(projName, projectFile, UsageType.Reference, link, projectType));
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

        #endregion
    }
}