using CopperGameTools.Builder;
using CopperGameTools.Shared;

namespace CopperGameTools.CGT;

internal abstract class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Logging.Print($"Please make sure to keep CGT updated to ensure it works with newer CopperCube Engine Versions.\n" +
                          $"At the time of this build, version {CopperGameToolsInfo.SupportedCopperCubeVersion} is the latest supported one.\n"+
                          $"CopperGameTools v{CopperGameToolsInfo.Version} | Build {CopperGameToolsInfo.BuildDate}\n\n" +
                          "No subcommand used.\n" + "Press any key to exit.", Logging.PrintLevel.Info);
            Console.ReadKey();
            return;
        }

        var filename = new StrongConstHolder<string>(GetProjectFilename(args));
        
        try
        {
            switch (args[0])
            {
                case "build":
                case "b":
                    HandleBuild(filename.Value);
                    break;
                case "check":
                case "c":
                    HandleCheck(filename.Value);
                    break;
                case "info":
                case "i":
                    HandleInfo(filename.Value);
                    break;
            }
        }
        catch (FileNotFoundException)
        {
            Logging.Print($"File {filename.Value} not found.", Logging.PrintLevel.Error);
        }
        catch (UnauthorizedAccessException)
        {
            Logging.Print($"Failed to access {filename.Value}", Logging.PrintLevel.Error);
        }
    }

    private static string GetProjectFilename(string[] args)
    {
        if (args.Length >= 2) 
            return args[1];
        string[] files = Directory.GetFiles("./", "*.cgt");
        if (files.Length != 0) 
            return files[0];
        Logging.Print("No project file found. Is there a .cgt-File in the current directory?", Logging.PrintLevel.Info);
        return "";
    }

    private static void HandleBuild(string filename)
    {
        ProjectBuilder builder = new(new ProjectFile(new FileInfo(filename)));
        ProjectFileCheckResult check = builder.ProjectFile.CheckProjectFile();
        ProjectBuilderResult result = builder.Build();
        switch (result.ResultType)
        {
            case ProjectBuilderResultType.DoneNoErrors:
                break;
            case ProjectBuilderResultType.FailedWithErrors:
                Logging.PrintErrors(check);
                break;
            case ProjectBuilderResultType.FailedWithProjectFileErrors:
                Logging.PrintErrors(check);
                break;
            default:
                Logging.Print("Detected an unexpected behaviour.", Logging.PrintLevel.Warning);
                Logging.PrintErrors(check);
                break;
        }
        
        Logging.WriteLog(CopperGameToolsInfo.BuildLogFilename);
    }

    private static void HandleCheck(string filename)
    {
        ProjectFileCheckResult check = 
            new ProjectFile(new FileInfo (filename)).CheckProjectFile();
        Logging.PrintErrors(check);
        
        Logging.Print($"Check result: {check.ResultType.ToString()}", Logging.PrintLevel.Info);
        
        Logging.WriteLog(CopperGameToolsInfo.CheckLogFilename);
    }

    private static void HandleInfo(string filename)
    {
        Logging.Print($"Reading project file {filename}", Logging.PrintLevel.Info);
        var file = new ProjectFile(new FileInfo(filename));
        if (file.SourceFile.DirectoryName == null)
        {
            Logging.Print($"Directory of {file} is null", Logging.PrintLevel.Error);
            return;
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
    }
}
