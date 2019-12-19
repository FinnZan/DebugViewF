﻿using System.Collections.Generic;

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

        public bool HasConflict
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
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }

    public class NugetReference
    {
        public NugetReference(string projName, string projectFile, string version)
        {
            ProjectName = projName;
            ProjecFile = projectFile;
            Version = version;
        }

        public string ProjectName
        {
            get;
        }

        public string ProjecFile
        {
            get;
        }

        public string Version
        {
            get;
        }
    }
}
