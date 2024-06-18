using CopperGameTools.Builder;
using CopperGameTools.Shared;
using System.Diagnostics;

namespace CopperGameTools.CLI;

class Program
{
    public static void Main(String[] args)
    {
        // No subcommand used / no args?
        if (Utils.ArrayIsEmpty(args))
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
            case "pack":
                if (args.Length < 2)
                {
                    Console.WriteLine("pack <project file>");
                    return;
                }
                try
                {
                    if (!Directory.Exists("Publish"))
                        Directory.CreateDirectory("./Publish");

                    ProjectFile projFile = new(new FileInfo(args[1]));

                    string platform = projFile.GetKey("project.platform");
                    if (platform == "")
                    {
                        Console.WriteLine("No platform specified!");
                        return;
                    }

                    string projectFileName = projFile.GetKey("project.file");
                    if (projectFileName == "" || !File.Exists(projectFileName))
                    {
                        Console.WriteLine("CopperCube-Project file not found!");
                        return;
                    }

                    string executableFileName = Path.GetFileNameWithoutExtension(projectFileName);
                    executableFileName += ".exe";

                    string packPath = "Publish/" + platform + "/";

                    if (Directory.Exists(packPath))
                    {
                        Directory.Delete(packPath);
                    }

                    Directory.CreateDirectory(packPath);

                    Console.WriteLine("Copy " + "./Project/" + executableFileName + " to " + packPath);
                    File.Copy("./Project/" + executableFileName, packPath + executableFileName);
                }
                catch (Exception)
                {
                    Console.WriteLine("ContentPacker: Failed to load file!");
                }
                break;
            case "build+":
                if (args.Length < 2)
                {
                    Console.WriteLine("build+ <project file>");
                    return;
                }
                try
                {
                    ProjectBuilder builder = new(new ProjectFile(new FileInfo(args[1])));
                    ProjectFileCheckResult check = builder.ProjectFile.CheckProjectFile();
                    switch (builder.Build())
                    {
                        case ProjectBuilderResultType.DoneNoErrors:
                            Console.WriteLine("No errors found.");
                            break;
                        case ProjectBuilderResultType.FailedWithErrors:
                            Console.WriteLine("Failed due to system errors!");
                            Utils.PrintErrors(check);
                            break;
                        case ProjectBuilderResultType.FailedWithProjectFileErrors:
                            Console.WriteLine("Failed due to project file errors!");
                            Utils.PrintErrors(check);
                            break;
                    }

                    string projectFileName = builder.ProjectFile.GetKey("project.file");
                    if (projectFileName == "" || !File.Exists(projectFileName))
                    {
                        Console.WriteLine("CopperCube-Project file not found!");
                        return;
                    }

                    string projectPlatform = builder.ProjectFile.GetKey("project.platform");
                    if (projectPlatform == "")
                    {
                        Console.WriteLine("No platform specified!");
                        return;
                    }

                    Console.WriteLine("Creating Game Executable from Project " + projectFileName + " for " + projectPlatform);

                    Process editor = new();

                    editor.StartInfo.FileName = "coppercube.exe";
                    editor.StartInfo.Arguments = $"{new FileInfo(projectFileName).FullName} -publish:{projectPlatform} -quit";
                    editor.StartInfo.CreateNoWindow = false;

                    editor.Start();
                    editor.WaitForExit();
                }
                catch (Exception)
                {
                    Console.WriteLine("Build+: Failed to load file!");
                }
                break;
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
                    switch (builder.Build())
                    {
                        case ProjectBuilderResultType.DoneNoErrors:
                            Console.WriteLine("No errors found.");
                            break;
                        case ProjectBuilderResultType.FailedWithErrors:
                            Console.WriteLine("Failed due to system errors!");
                            Utils.PrintErrors(check);
                            break;
                        case ProjectBuilderResultType.FailedWithProjectFileErrors:
                            Console.WriteLine("Failed due to project file errors!");
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
