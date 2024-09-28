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

        switch (args[0])
        {
            case "build":
            case "b":
                if (args.Length < 2)
                {
                    string[] files = Directory.GetFiles("./", "*.cgt");
                    if (files.Length == 0)
                    {
                        Logging.Print("No project file found.", Logging.PrintLevel.Info);
                        return;
                    }
                    HandleBuild(files[0]);
                }
                else
                {
                    HandleBuild(args[1]);
                }
                break;
            case "check":
            case "c":
                if (args.Length < 2)
                {
                    string[] files = Directory.GetFiles("./", "*.cgt");
                    if (files.Length == 0)
                    {
                        Logging.Print("No project file found.", Logging.PrintLevel.Info);
                        return;
                    }
                    HandleCheck(files[0]);
                }
                else
                {
                    HandleCheck(args[1]);
                }
                break;
            case "info":
            case "i":
                if (args.Length < 2)
                {
                    string[] files = Directory.GetFiles("./", "*.cgt");
                    if (files.Length == 0)
                    {
                        Logging.Print("No project file found.", Logging.PrintLevel.Info);
                        return;
                    }
                    HandleInfo(files[0]);
                }
                else
                {
                    HandleInfo(args[1]);
                }
                break;
        }
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
                Logging.Print("Detected an unexpected behaviour.", Logging.PrintLevel.Warn);
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
        
        Logging.WriteLog("check-latest.log");
    }

    private static void HandleInfo(string filename)
    {
        Logging.Print($"Reading project file {filename}", Logging.PrintLevel.Info);
        var file = new ProjectFile(new FileInfo(filename));
        Logging.PrintErrors(file.CheckProjectFile());
        
        string version = file.GetKey("builder.version");
        bool isCompatible = version is CopperGameToolsInfo.Version or CopperGameToolsInfo.MajorVersion;
        if (!version.Equals(""))
            Logging.Print($"CGT Version: {version} | Compatible with installation: {isCompatible}", Logging.PrintLevel.Info);
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
