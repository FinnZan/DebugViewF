namespace ReferenceViewer
{
    public class AssemblyReference
    {
        public AssemblyReference(string projectName, string projectFile, UsageType usage, string refPath, ProjectType projectType)
        {
            ProjectName = projectName;
            ProjecFile = projectFile;
            Usage = usage;
            ReferencePath = refPath;
            ProjectType = projectType;
        }

        public string ProjectName
        {
            get;
        }

        public string ProjecFile
        {
            get;
        }

        public ProjectType ProjectType 
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