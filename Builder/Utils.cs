namespace CopperGameTools.Builder
{
    public class Utils
    {
        public static string GetVersion() { return "0.4.5"; }
        public static string GetBuildDate() { return "16.03.24 [DD.MM.YY]"; }
        public static string GetCopyright() { return "AGBgames 2024"; }

        public static void PrintErrors(ProjFileCheckResult projFileCheckResult)
        {
            Console.WriteLine("Number of Errors: " + projFileCheckResult.ResultErrors.Count);
            foreach (var err in projFileCheckResult.ResultErrors)
            {
                Console.WriteLine($"{err.ErrorText} | Error Type: {err.ErrorType}");
            }
        }
    }
}
