using CopperGameTools.Builder;
using CopperGameTools.Shared;

namespace CopperGameTools.CGT;

internal abstract class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Utils.Print($"Please make sure to keep CGT updated to ensure it works with newer CopperCube Engine Versions.\n" +
            $"At the time of this build, version {Constants.SupportedCopperCubeVersion} is the latest supported one.\n"+
            $"CopperGameTools v{Constants.Version} | Build {Constants.BuildDate}\n" +
            "No subcommand used.\n" + "Press any key to exit.", Utils.PrintLevel.Info);
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
                        Console.WriteLine("build <project file>");
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
                        Console.WriteLine("build <project file>");
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
                Utils.PrintErrors(check);
                break;
            case ProjectBuilderResultType.FailedWithProjectFileErrors:
                Utils.PrintErrors(check);
                break;
        }
    }

    private static void HandleCheck(string filename)
    {
        ProjectFileCheckResult check = 
            new ProjectFile(new FileInfo (filename)).CheckProjectFile();
        Utils.PrintErrors(check);
    }
}
