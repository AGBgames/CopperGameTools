using CopperGameTools.Builder;
using CopperGameTools.Shared;

namespace CopperGameTools.CGT.Commands
{
    internal class InfoCommand : ICommand
    {
        public string Parameter()
        {
            return "info";
        }

        public string Alias()
        {
            return "i";
        }

        public bool Execute(string filename)
        {
            Logging.Print($"Reading project file {filename}", Logging.PrintLevel.Info);
            var file = new ProjectFile(new FileInfo(filename));
            if (file.SourceFile.DirectoryName == null)
            {
                Logging.Print($"Directory of {file} is null", Logging.PrintLevel.Error);
                return false;
            }
            Logging.Print("Checking project file: ", Logging.PrintLevel.Info);
            Logging.PrintErrors(file.CheckProjectFile());

            string version = file.GetKey(ProjectFileKeys.BuilderVersion);
            bool isCompatible = version is CopperGameToolsInfo.Version or CopperGameToolsInfo.MajorVersion;
            if (!version.Equals(ProjectFileKeys.InvalidKey))
                Logging.Print($"Projects CGT Version: {version} | Compatible with installation: {isCompatible}", Logging.PrintLevel.Info);
            bool requiresVersion = file.GetKeyAsBoolean(ProjectFileKeys.BuilderRequireVersion);
            Logging.Print($"Requires specific CGT Version: {requiresVersion}", Logging.PrintLevel.Info);

            string projectName = file.GetKey(ProjectFileKeys.ProjectName);
            if (!projectName.Equals(ProjectFileKeys.InvalidKey))
                Logging.Print($"Project name: {projectName}", Logging.PrintLevel.Info);

            string sourceDir = Path.Combine(file.SourceFile.DirectoryName, file.GetKey(ProjectFileKeys.ProjectSourceDirectory));
            if (!sourceDir.Equals(ProjectFileKeys.InvalidKey))
            {
                if (Directory.Exists(sourceDir))
                    Logging.Print($"Source directory: {sourceDir}", Logging.PrintLevel.Info);
            }

            string sourceOut = file.GetKey(ProjectFileKeys.ProjectOutputFilename);
            if (!sourceOut.Equals(ProjectFileKeys.InvalidKey))
            {
                if (File.Exists(sourceOut))
                    Logging.Print($"Output name: {sourceOut}", Logging.PrintLevel.Info);
            }

            string outDir = Path.Combine(file.SourceFile.DirectoryName, file.GetKey(ProjectFileKeys.ProjectOutputDirectory));
            if (!outDir.Equals(ProjectFileKeys.InvalidKey))
            {
                if (Directory.Exists(outDir))
                    Logging.Print($"Output directory: {outDir}", Logging.PrintLevel.Info);
            }

            Logging.WriteLog(CopperGameToolsInfo.InfoLogFilename);

            return true;
        }
    }
}
