using CopperGameTools.Builder;
using CopperGameTools.Shared;

namespace CopperGameTools.CGT.Commands
{
    internal class CheckCommand : ICommand
    {
        public string Parameter()
        {
            return "check";
        }

        public string Alias()
        {
            return "c";
        }

        public bool Execute(string filename)
        {
            ProjectFileCheckResult check =
            new ProjectFile(new FileInfo(filename)).CheckProjectFile();
            Logging.PrintErrors(check);

            Logging.Print($"Check result: {check.ResultType.ToString()}", Logging.PrintLevel.Info);

            Logging.WriteLog(CopperGameToolsInfo.CheckLogFilename);

            return true;
        }
    }
}
