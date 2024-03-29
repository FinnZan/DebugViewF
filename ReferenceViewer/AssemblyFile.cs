﻿using System;
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
}