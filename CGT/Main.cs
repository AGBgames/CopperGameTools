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
                $"At the time of this build, version {Shared.CopperGameToolsInfo.SupportedCopperCubeVersion} is the latest supported one.\n"+
                $"CopperGameTools v{Shared.CopperGameToolsInfo.Version} | Build {Shared.CopperGameToolsInfo.BuildDate}\n\n" +
                "No subcommand used.\n" + "Press any key to exit.", Logging.PrintLevel.Info);
            Console.ReadKey();
            return;
        }

        string file = GetProjectFile(args);

        if (!File.Exists(file) || string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file))
        {
            Logging.Print($"File {file} is not valid.", Logging.PrintLevel.Error);
            return;
        }

        switch (args[0])
        {
            case "build":
            case "b":
                HandleBuild(file);
                break;
            case "check":
            case "c":
                HandleCheck(file);
                break;
            case "info":
            case "i":
                HandleInfo(file);
                break;
        }
    }

    private static string GetProjectFile(string[] args)
    {
        if (args.Length < 2)
        {
            string[] files = Directory.GetFiles("./", "*.cgt");
            if (files.Length != 0) return files[0];
            Logging.Print("No project file found.", Logging.PrintLevel.Info);
            return "";
        }
        return args[1];
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
                Logging.Print($"Cause: {result.ResultCauseInformation}", Logging.PrintLevel.Info);
                break;
            case ProjectBuilderResultType.FailedWithProjectFileErrors:
                Logging.PrintErrors(check);
                Logging.Print($"Cause: {result.ResultCauseInformation}", Logging.PrintLevel.Info);
                break;
            default:
                Logging.Print("Detected an unexpected behaviour.", Logging.PrintLevel.Warning);
                Logging.PrintErrors(check);
                break;
        }
        
        
        Logging.WriteLog("build-latest.log");
    }

    private static void HandleCheck(string filename)
    {
        ProjectFileCheckResult check = 
            new ProjectFile(new FileInfo (filename)).CheckProjectFile();
        Logging.PrintErrors(check);
        
        Logging.Print($"Check result: {check.ResultType.ToString()}", Logging.PrintLevel.Info);
        
        Logging.WriteLog("check-latest.log");
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
        
        string version = file.GetKey("builder.version");
        bool isCompatible = version is CopperGameToolsInfo.Version or CopperGameToolsInfo.MajorVersion;
        if (!version.Equals(""))
            Logging.Print($"Projects CGT Version: {version} | Compatible with installation: {isCompatible}", Logging.PrintLevel.Info);
        bool requiresVersion = file.GetKeyAsBoolean("builder.require_version", false);
        Logging.Print($"Requires specific CGT Version: {requiresVersion}", Logging.PrintLevel.Info);
        
        string projectName = file.GetKey("project.name");
        if (!projectName.Equals(""))
            Logging.Print($"Project name: {projectName}", Logging.PrintLevel.Info);
        string sourceDir = Path.Combine(file.SourceFile.DirectoryName, file.GetKey("project.src.dir"));
        if (!sourceDir.Equals(""))
        {
            if (Directory.Exists(sourceDir))
                Logging.Print($"Source directory: {sourceDir}", Logging.PrintLevel.Info);
        }
        string sourceOut = file.GetKey("project.src.out");
        if (!sourceOut.Equals(""))
        {
            if (Directory.Exists(sourceOut))
                Logging.Print($"Output name: {sourceOut}", Logging.PrintLevel.Info);
        }
        string outDir = Path.Combine(file.SourceFile.DirectoryName, file.GetKey("project.out.dir"));
        if (!outDir.Equals(""))
        {
            if (Directory.Exists(outDir))
                Logging.Print($"Output directory: {outDir}", Logging.PrintLevel.Info);
        }
        
        Logging.WriteLog("info-latest.log");
    }
}
