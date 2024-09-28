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
                $"At the time of this build, version {Constants.SupportedCopperCubeVersion} is the latest supported one.\n"+
                $"CopperGameTools v{Constants.Version} | Build {Constants.BuildDate}\n\n" +
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
        }
        
        if (!Directory.Exists("./.cgt/"))
            Directory.CreateDirectory("./.cgt/");
        File.WriteAllText("./.cgt/latest.log", Logging.Log);
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
    }

    private static void HandleCheck(string filename)
    {
        ProjectFileCheckResult check = 
            new ProjectFile(new FileInfo (filename)).CheckProjectFile();
        Logging.PrintErrors(check);
    }
}
