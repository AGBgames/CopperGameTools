using CopperGameTools.Builder;
using System.Diagnostics;

namespace CopperGameTools.CLI;

class Program {
    public static void Main(String[] args) {
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
                Console.WriteLine("CopperGameTools Command Line Interface v0.3");
                break;
            case "coppui":
                Process uiProc = new Process();
                uiProc.StartInfo.FileName = "CopperGameTools.CopperUI.exe";
                uiProc.Start();
                uiProc.WaitForExit();
                break;
            case "build":
                if (args.Length < 2) return;
                CGTProjBuilder builder = new CGTProjBuilder(new CGTProjFile(new FileInfo(args[1])));
                switch (builder.Build().ResultType)
                {
                    case CGTProjBuilderResultType.DoneNoErrors:
                    Console.WriteLine("No errors found.");
                    break;
                case CGTProjBuilderResultType.DoneWithErrors:
                    Console.WriteLine("Errors found.");
                    builder.ProjFile.PrintErros();
                    break;
                case CGTProjBuilderResultType.FailedNoErrors:
                    Console.WriteLine("Failed with no errors.");
                    break;
                case CGTProjBuilderResultType.FailedWithErrors:
                    Console.WriteLine("Failed with errors");
                    builder.ProjFile.PrintErros();
                    break;
                }
                break;
            case "checkpkf":
                CGTProjFileCheckResult checkRes = new CGTProjBuilder(new CGTProjFile(new FileInfo(args[1]))).ProjFile.FileCheck();
                foreach (var err in checkRes.ResultErrors)
                {
                    Console.WriteLine($"{err.ErrorText} | Type => {err.ErrorType} | Is Critical => {err.IsCritical}\n");
                }
                break;
        }
    }
}
