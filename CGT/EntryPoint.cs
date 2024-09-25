using CopperGameTools.Builder;
using CopperGameTools.Shared;

namespace CopperGameTools.CGT;

internal abstract class Program
{
    public static void Main(String[] args)
    {
        // No subcommand used / no args?
        if (args.Length == 0)
        {
            Console.WriteLine($"Please make sure to keep CGT updated to ensure it works with newer CopperCube Engine Versions.\n" +
            $"At the time of this build, version {Constants.SupportedCopperCubeVersion} is the latest supported one.");
            Console.WriteLine($"CopperGameTools v{Constants.Version} | Build {Constants.BuildDate}\n" +
                "No subcommand used.\n");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return;
        }

        switch (args[0])
        {
            case "build":
                if (args.Length < 2)
                {
                    Console.WriteLine("build <project file>");
                    return;
                }
                try
                {
                    ProjectBuilder builder = new(new ProjectFile(new FileInfo(args[1])));
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
                catch (Exception)
                {
                    Console.WriteLine("Build: Failed to load file!");
                }
                break;

            case "check":
                if (args.Length < 2)
                {
                    Console.WriteLine("check <project file>");
                    return;
                }
                try
                {
                    ProjectFileCheckResult check = 
                        new ProjectFile(new FileInfo (args[1])).CheckProjectFile();
                    Utils.PrintErrors(check);
                }
                catch (Exception)
                {
                    Console.WriteLine("Check: Failed to load file!");
                }
                break;
        }
    }
}
