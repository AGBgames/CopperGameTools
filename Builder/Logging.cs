namespace CopperGameTools.Builder;

public abstract class Logging
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

    private static string Log { get; set; } = "";

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

        string fullMessage = "";
        
        switch (printLevel)
        {
            case PrintLevel.Info:
                fullMessage = $"[Info/{time}]: {message}";
                Console.WriteLine(fullMessage);
                break;
            case PrintLevel.Warn:
                fullMessage = $"[Warn/{time}]: {message}";
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(fullMessage);
                break;
            case PrintLevel.Error:
                fullMessage = $"[Error/{time}]: {message}";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(fullMessage);
                break;
        }
        Log += fullMessage + "\n";
        Console.ResetColor();
    }

    public static void WriteLog(string filename)
    {
        if (!Directory.Exists("./.cgt/"))
            Directory.CreateDirectory("./.cgt/");
        File.WriteAllText($"./.cgt/{filename}", Logging.Log);
    }
}
