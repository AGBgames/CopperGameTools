using CopperGameTools.Builder;

namespace CopperGameTools.CLI;

class Program {
    public static void Main(String[] args) {
        if (args.Length == 0) return;

        var builder = new CGTProjBuilder(new CGTProjFile(new FileInfo(args[0])));
        
        var build = builder.Build();
        switch (build.ResultType)
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
    }
}