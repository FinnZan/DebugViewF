using System;
using System.Collections.Generic;
using System.IO;

namespace ReferenceViewer
{
    public class AssemblyFile
    {
        public AssemblyFile(string path, DateTime time)
        {
            FullPath = path;
            Projects = new List<Project>();
            LastModified = time;
        }

        public DateTime LastModified { get; }
        
        public string Name => Path.GetFileName(FullPath);

        public string FullPath { get; }

        public List<Project> Projects { get; }
    }

    public enum UsageType
    {
        Reference,
        Link
    }


    public class Project
    {
        public Project() { }

        public string Name{
            get;
            set;
        }

        public UsageType Usage
        {
            get;
            set;
        }
    }
}