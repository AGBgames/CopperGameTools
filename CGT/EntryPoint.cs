using CopperGameTools.Builder;
using System.Diagnostics;

namespace CopperGameTools.CLI
{
    class Program
    {
        public static void Main(String[] args)
        {
            Console.WriteLine($"Please make sure to keep CGT updated to ensure it works with newer CopperCube Engine Versions.\n" +
                $"At the time of this build, version {Shared.Constants.SupportedCopperCubeVersion} is the latest supported one.");

            // No subcommand used / no args?
            if (Utils.IsEmpty(args))
            {
                Console.WriteLine($"CopperGameTools v{Shared.Constants.Version} on {Shared.Constants.BuildDate}\n" +
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
                        Console.WriteLine("Failed to load file!");
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
                        ProjFileCheckResult check = builder.ProjectFile.CheckProjectFile();
                        switch (builder.Build().ResultType)
                        {
                            case ProjectBuilderResultType.DoneNoErrors:
                                Console.WriteLine("Error: Not caused by project-file!");
                                break;
                            case ProjectBuilderResultType.FailedNoErrors:
                                Console.WriteLine("Failed: Not caused by project-file!");
                                break;
                            case ProjectBuilderResultType.FailedWithErrors:
                                Console.WriteLine("Failed: Caused by project-file!");
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
                        Console.WriteLine("Failed to load file!");
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
                        ProjFileCheckResult check = builder.ProjectFile.CheckProjectFile();
                        switch (builder.Build().ResultType)
                        {
                            case ProjectBuilderResultType.DoneNoErrors:
                                Console.WriteLine("No errors found.");
                                break;
                            case ProjectBuilderResultType.FailedNoErrors:
                                Console.WriteLine("Failed with no errors.");
                                break;
                            case ProjectBuilderResultType.FailedWithErrors:
                                Console.WriteLine("Failed with errors");
                                Utils.PrintErrors(check);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to load file!");
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
                        ProjFileCheckResult check = new ProjectBuilder(new ProjectFile(new FileInfo(args[1]))).ProjectFile.CheckProjectFile();
                        Utils.PrintErrors(check);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to load file!");
                    }
                    break;
            }
        }
    }

}
