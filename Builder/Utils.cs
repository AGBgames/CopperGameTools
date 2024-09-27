namespace CopperGameTools.Builder;

public static class Utils
{
    public static void PrintErrors(ProjectFileCheckResult projectFileCheckResult)
    {
        if (projectFileCheckResult.ResultErrors.Count == 0)
        {
            Print("No errors found.", PrintLevel.Info);
            return;
        }
        Print("Number of Errors: " + projectFileCheckResult.ResultErrors.Count, PrintLevel.Warn);
        foreach (ProjectFileCheckError err in projectFileCheckResult.ResultErrors)
        {
            Print($"{err.ErrorText} | Error Type: {err.ErrorType}", PrintLevel.Error);
        }
    }

    public enum PrintLevel
    {
        Info,
        Warn,
        Error,
    }
    
    public static void Print(string message, PrintLevel printLevel)
    {
        DateTime now = DateTime.Now;
        string time = now.ToString("hh:mm");
        
        switch (printLevel)
        {
            case PrintLevel.Info:
                Console.WriteLine($"[Info/{time}]: {message}");
                break;
            case PrintLevel.Warn:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Warn/{time}]: {message}");
                break;
            case PrintLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error/{time}]: {message}");
                break;
        }
        Console.ResetColor();
    }
}
