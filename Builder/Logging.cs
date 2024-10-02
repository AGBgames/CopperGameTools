namespace CopperGameTools.Builder;

public abstract class Logging
{
    private static string Log { get; set; } = "";
    
    public static void PrintErrors(ProjectFileCheckResult projectFileCheckResult)
    {
        if (projectFileCheckResult.ResultErrors.Count == 0)
        {
            Print("No errors found.", PrintLevel.Info);
            return;
        }
        Print("Number of Errors: " + projectFileCheckResult.ResultErrors.Count, PrintLevel.Warning);
        foreach (ProjectFileCheckError err in projectFileCheckResult.ResultErrors)
        {
            Print($"{err.ErrorText} | Error Type: {err.ErrorType}", PrintLevel.Error);
        }
    }

    public enum PrintLevel
    {
        Info,
        Warning,
        Error
    }
    
    public static void Print(string message, PrintLevel printLevel)
    {
        DateTime now = DateTime.Now;
        string time = now.ToString("hh:mm");

        string fullMessage;
        
        switch (printLevel)
        {
            case PrintLevel.Info:
                fullMessage = $"[Info/{time}]: {message}";
                break;
            case PrintLevel.Warning:
                fullMessage = $"[Warn/{time}]: {message}";
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case PrintLevel.Error:
                fullMessage = $"[Error/{time}]: {message}";
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                fullMessage = $"[Unknown/{time}]: {message}";
                break;
        }
        Console.WriteLine(fullMessage);
        Log += fullMessage + "\n";
        Console.ResetColor();
    }

    public static void WriteLog(string filename)
    {
        if (!Directory.Exists("./.cgt/"))
            Directory.CreateDirectory("./.cgt/");
        File.WriteAllText($"./.cgt/{filename}", Log);
    }
}
