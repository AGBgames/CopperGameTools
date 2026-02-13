using CopperGameTools.CGT.Commands;
using CopperGameTools.Builder;
using CopperGameTools.Shared;

namespace CopperGameTools.CGT;

abstract class Program
{
    private static readonly List<ICommand> _commands = [new BuildCommand(), new CheckCommand(), new InfoCommand()];

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Logging.Print($"Please make sure to keep CGT updated to ensure it works with newer CopperCube Engine Versions.\n" +
                          $"At the time of this build, version {CopperGameToolsInfo.SupportedCopperCubeVersion} is the latest supported one.\n"+
                          $"CopperGameTools v{CopperGameToolsInfo.Version} | Build {CopperGameToolsInfo.BuildDate}\n\n" +
                          "No subcommand used.\n" + "Press any key to exit.", Logging.PrintLevel.Info);
            Console.ReadKey();
            return;
        }

        string filename = GetProjectFilename(args);
        string usedCommandParameter = args[0];
        
        try
        {
            // ICommand? command = _commands
            //     .Where(command =>
            //     string.Equals(command.Parameter(), usedCommandParameter.Value(), StringComparison.OrdinalIgnoreCase)
            //     || string.Equals(command.Alias(), usedCommandParameter.Value(), StringComparison.OrdinalIgnoreCase))
            //     .FirstOrDefault();
            
            ICommand? command = _commands
                .FirstOrDefault(command => string.Equals(command.Parameter(), usedCommandParameter, StringComparison.OrdinalIgnoreCase)
                                           || string.Equals(command.Alias(), usedCommandParameter, StringComparison.OrdinalIgnoreCase));

            if (command != null)
            {
                if (!command.Execute(filename))
                    Logging.Print("The process was ended unexpectedly.", Logging.PrintLevel.Warning);
            }
            else
            {
                Logging.Print("Command not found.", Logging.PrintLevel.Warning);
            }
        }
        catch (FileNotFoundException)
        {
            Logging.Print($"File {filename} not found.", Logging.PrintLevel.Error);
        }
        catch (UnauthorizedAccessException)
        {
            Logging.Print($"Failed to access {filename}", Logging.PrintLevel.Error);
        }
    }

    private static string GetProjectFilename(string[] args)
    {
        if (args.Length >= 2) 
            return args[1];
        string[] files = Directory.GetFiles("./", "*.cgt");
        if (files.Length != 0)
            return files[0];
        Logging.Print("No project file found. Is there a .cgt-File in the current directory?", Logging.PrintLevel.Info);
        return "";
    }
}
