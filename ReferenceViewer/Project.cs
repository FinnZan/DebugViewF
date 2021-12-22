using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceViewer
{
    public class Project
    {
        public Project(string actualPath) 
        {
            ActualPath = actualPath;
            Projects = new List<AssemblyReference>();
        }

        public string Name => Path.GetFileName(ActualPath);

        public string ActualPath { get; }

        public List<AssemblyReference> Projects { get; }
    }
}
