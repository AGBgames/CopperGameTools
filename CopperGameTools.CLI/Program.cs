using CopperGameTools.Builder;

namespace CopperGameTools.CLI;

class Program {
    public static void Main(String[] args) {
        if (args.Length == 0)
        {
            System.Console.WriteLine("No subcommand used. \n");
            System.Console.WriteLine(
                    "uicl / uiclient - boots up CopperUI. \n" + 
                    "build - builds a .PKF-File. \n" + 
                    "checkpkf - checks a .PKF-File.\n" + 
                    "info - shows info about the CLI and CopperGameToools."
            );
            return;
        }

        switch (args[0])
        {
            case "info":
                System.Console.WriteLine("CopperGameTools Command Line Interface v0.232.1");
                break;
            case "uicl":
            case "uiclient":
                var uiProc = new System.Diagnostics.Process();
                uiProc.StartInfo.FileName = "CopperGameTools.CopperUI.exe";
                uiProc.Start();
                uiProc.WaitForExit();
                break;
            case "build":
                if (args.Length < 2) return;
                var builder = new CGTProjBuilder(new CGTProjFile(new FileInfo(args[1])));
                switch (builder.Build().ResultType)
                {
                    case CGTProjBuilderResultType.DoneNoErrors:
                    System.Console.WriteLine("No errors found.");
                    break;
                case CGTProjBuilderResultType.DoneWithErrors:
                    System.Console.WriteLine("Errors found.");
                    builder.ProjFile.PrintErros();
                    break;
                case CGTProjBuilderResultType.FailedNoErrors:
                    System.Console.WriteLine("Failed with no errors.");
                    break;
                case CGTProjBuilderResultType.FailedWithErrors:
                    System.Console.WriteLine("Failed with errors");
                    builder.ProjFile.PrintErros();
                    break;
                }
                break;
            case "checkpkf":
                var checkRes = new CGTProjBuilder(new CGTProjFile(new FileInfo(args[1]))).ProjFile.FileCheck();
                foreach (var err in checkRes.ResultErrors)
                {
                    System.Console.WriteLine($"{err.ErrorText} | Type => {err.ErrorType} | Is Critical => {err.IsCritical}\n");
                }
                break;
        }
    }
}
