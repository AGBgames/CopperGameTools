namespace CopperGameTools.Builder;

//TODO: move Logging into Shared Project

/// <summary>
/// Logging utility class.
/// </summary>
public abstract class Logging
{
    /// <summary>
    /// Holds the current log string. 
    /// </summary>
    private static string Log { get; set; } = "";
    
    /// <summary>
    /// Prints projectfile errors in a format.
    /// </summary>
    /// <param name="projectFileCheckResult">The ProjectFileCheckResults which will provide the list of errors to print.</param>
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

    /// <summary>
    /// All the different logging levels.
    /// </summary>
    public enum PrintLevel
    {
        Info,
        Warning,
        Error
    }
    
    /// <summary>
    /// Print and append to log string.
    /// </summary>
    /// <param name="message">What to print.</param>
    /// <param name="printLevel">Log level of the message.</param>
    public static void Print(string message, PrintLevel printLevel)
    {
        DateTime now = DateTime.Now;
        string time = now.ToString("HH:mm");

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

    /// <summary>
    /// Write the log string to a file.
    /// </summary>
    /// <param name="filename">Filename for the log file.</param>
    public static void WriteLog(string filename)
    {
        if (!Directory.Exists("./.cgt/"))
            Directory.CreateDirectory("./.cgt/");
        File.WriteAllText($"./.cgt/{filename}", Log);
    }
}
