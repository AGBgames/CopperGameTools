using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopperGameTools.Builder
{
    public class Utils
    {
        public static string GetVersion() { return "0.4.2 [25.05.23 / DD.MM.YY]"; }
        public static string GetCopyright() { return "AGBgames 2023"; }

        public static void PrintErrors(ProjFileCheckResult projFileCheckResult)
        {
            Console.WriteLine("Number of Errors: " + projFileCheckResult.ResultErrors.Count());
            foreach (var err in projFileCheckResult.ResultErrors)
            {
                Console.WriteLine(
                    $"{err.ErrorText} | ErrorType: {err.ErrorType} | Is Critical: {err.IsCritical}"
                );
            }
        }
    }
}
