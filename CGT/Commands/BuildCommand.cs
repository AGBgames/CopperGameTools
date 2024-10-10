using CopperGameTools.Builder;
using CopperGameTools.Shared;

namespace CopperGameTools.CGT.Commands
{
    public class BuildCommand : ICommand
    {
        public string Parameter()
        {
            return "build";
        }

        public string Alias()
        {
            return "b";
        }

        public bool Execute(string filename)
        {
            ProjectBuilder builder = new(new ProjectFile(new FileInfo(filename)));
            ProjectFileCheckResult check = builder.ProjectFile.CheckProjectFile();
            ProjectBuilderResult result = builder.Build();
            switch (result.ResultType)
            {
                case ProjectBuilderResultType.DoneNoErrors:
                    break;
                case ProjectBuilderResultType.FailedWithErrors:
                    Logging.PrintErrors(check);
                    break;
                case ProjectBuilderResultType.FailedWithProjectFileErrors:
                    Logging.PrintErrors(check);
                    break;
                default:
                    Logging.Print("Detected an unexpected behaviour.", Logging.PrintLevel.Warning);
                    Logging.PrintErrors(check);
                    break;
            }

            Logging.WriteLog(CopperGameToolsInfo.BuildLogFilename);

            return true;
        }
    }
}
