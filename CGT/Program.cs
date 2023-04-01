using CopperGameTools.Builder;
using System.Diagnostics;

namespace CopperGameTools.CLI
{
    class Program
    {
        public static void Main(String[] args)
        {
            // No subcommand used / no args?
            if (args.Length == 0)
            {
                Console.WriteLine("No subcommand used. \n");
                Console.WriteLine(
                        "build - builds a .PKF-File. \n" +
                        "checkpkf - checks a .PKF-File.\n" +
                        "info - shows info about the CLI and CopperGameToools."
                );
                return;
            }

            switch (args[0])
            {
                case "info":
                    Console.WriteLine("CopperGameTools v0.4.1");
                    break;
                case "build":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("build <pkf-file>");
                        return;
                    }
                    try
                    {
                        ProjBuilder builder = new ProjBuilder(new ProjFile(new FileInfo(args[1])));
                        switch (builder.Build().ResultType)
                        {
                            case ProjBuilderResultType.DoneNoErrors:
                                Console.WriteLine("No errors found.");
                                break;
                            case ProjBuilderResultType.DoneWithErrors:
                                Console.WriteLine("Errors found.");
                                builder.ProjFile.PrintErros();
                                break;
                            case ProjBuilderResultType.FailedNoErrors:
                                Console.WriteLine("Failed with no errors.");
                                break;
                            case ProjBuilderResultType.FailedWithErrors:
                                Console.WriteLine("Failed with errors");
                                builder.ProjFile.PrintErros();
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to load file!");
                    }
                    break;
                case "checkpkf":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("checkpkf <pkf-file>");
                        return;
                    }
                    try
                    {
                        ProjFileCheckResult checkRes = new ProjBuilder(new ProjFile(new FileInfo(args[1]))).ProjFile.FileCheck();
                        foreach (var err in checkRes.ResultErrors)
                        {
                            Console.WriteLine($"{err.ErrorText} | Type => {err.ErrorType} | Is Critical => {err.IsCritical}\n");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to load file!");
                    }
                    break;
            }
        }
    }

}