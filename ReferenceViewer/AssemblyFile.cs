using System;
using System.Collections.Generic;
using System.IO;

namespace ReferenceViewer
{
    public class AssemblyFile
    {
        public AssemblyFile(string actualPath, DateTime time, bool isLocal)
        {
            ActualPath = actualPath;
            Projects = new List<AssemblyReference>();
            LastModified = time;
            IsLocal = isLocal;
        }

        public DateTime LastModified { get; }

        public string Name => Path.GetFileName(ActualPath);

        public string ActualPath { get; }

        public List<AssemblyReference> Projects { get; }

        public bool IsLocal 
        {
            get;
        }
    }

    public enum UsageType
    {
        Reference,
        Link
    }
    
    public class AssemblyReference
    {
        public AssemblyReference(string projectName, string projectFile, UsageType usage, string refPath)
        {
            ProjectName = projectName;
            ProjecFile = projectFile;
            Usage = usage;
            ReferencePath = refPath;
        }

        public string ProjectName
        {
            get;
        }

        public string ProjecFile
        {
            get;
        }

        public string ReferencePath { get; }

        public UsageType Usage
        {
            get;
        }
    }
}