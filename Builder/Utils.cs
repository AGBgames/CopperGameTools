namespace CopperGameTools.Builder
{
    public static class Utils
    {
        public static void PrintErrors(ProjFileCheckResult projFileCheckResult)
        {
            Console.WriteLine("Number of Errors: " + projFileCheckResult.ResultErrors.Count);
            foreach (var err in projFileCheckResult.ResultErrors)
            {
                Console.WriteLine($"{err.ErrorText} | Error Type: {err.ErrorType}");
            }
        }

        public static bool IsEmpty(Array array)
        {
            return array.Length == 0;
        }
    }
}
