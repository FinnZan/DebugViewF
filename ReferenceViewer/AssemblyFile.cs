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
            Projects = new List<string>();
            LastModified = time;
        }

        public DateTime LastModified { get; }
        
        public string Name => Path.GetFileName(FullPath);

        public string FullPath { get; }

        public List<string> Projects { get; }
    }
}