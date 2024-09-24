namespace CopperGameTools.Builder;

public static class Utils
{
    public static void PrintErrors(ProjectFileCheckResult projectFileCheckResult)
    {
        Console.WriteLine("Number of Errors: " + projectFileCheckResult.ResultErrors.Count);
        foreach (ProjectFileCheckError err in projectFileCheckResult.ResultErrors)
        {
            Console.WriteLine($"{err.ErrorText} | Error Type: {err.ErrorType}");
        }
    }

    public static bool ArrayIsEmpty(Array array)
    {
        return array.Length == 0;
    }
}
