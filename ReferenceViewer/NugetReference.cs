namespace ReferenceViewer
{
    public class NugetReference
    {
        public NugetReference(string projName, string projectFile, string version, ProjectType projectType)
        {
            ProjectName = projName;
            ProjecFile = projectFile;
            Version = version;
            ProjectType = projectType;
        }

        public string ProjectName
        {
            get;
        }

        public ProjectType ProjectType
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
