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
                Console.WriteLine("CopperGameTools v0.4.1 \n" +
                    "No subcommand used.\n");
                return;
            }

            switch (args[0])
            {
                case "build":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("build <pkf-file>");
                        return;
                    }
                    try
                    {
                        ProjBuilder builder = new ProjBuilder(new ProjFile(new FileInfo(args[1])));
                        ProjFileCheckResult projFileCheckResult = builder.ProjFile.FileCheck();
                        switch (builder.Build().ResultType)
                        {
                            case ProjBuilderResultType.DoneNoErrors:
                                Console.WriteLine("No errors found.");
                                break;
                            case ProjBuilderResultType.DoneWithErrors:
                                Console.WriteLine("Errors found.");
                                Utils.PrintErrors(projFileCheckResult);
                                break;
                            case ProjBuilderResultType.FailedNoErrors:
                                Console.WriteLine("Failed with no errors.");
                                break;
                            case ProjBuilderResultType.FailedWithErrors:
                                Console.WriteLine("Failed with errors");
                                Utils.PrintErrors(projFileCheckResult);
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
                        Console.WriteLine("check <pkf-file>");
                        return;
                    }
                    try
                    {
                        ProjFileCheckResult checkRes = new ProjBuilder(new ProjFile(new FileInfo(args[1]))).ProjFile.FileCheck();
                        Utils.PrintErrors(checkRes);
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