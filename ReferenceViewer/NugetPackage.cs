using System.Collections.Generic;

namespace ReferenceViewer
{
    public class NugetPackage
    {
        public NugetPackage(string name)
        {
            Name = name;
            Projects = new List<NugetReference>();
        }

        public string Name
        {
            get;
        }

        public List<NugetReference> Projects
        {
            get;
        }

        public bool IsConsistent
        {
            get
            {
                string last = null;
                foreach(var p in Projects)
                {
                    if(last == null)
                    {
                        last = p.Version;
                    }
                    else
                    {
                        if(last != p.Version)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
