using CopperGameTools.CGT.Commands;
using CopperGameTools.Builder;
using CopperGameTools.Shared;

namespace CopperGameTools.CGT;

internal abstract class Program
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

        var filename = new StrongReadOnlyHolder<string>(GetProjectFilename(args));
        var usedCommandParameter = new StrongReadOnlyHolder<string>(args[0]);
        
        try
        {
            var command = _commands
                .Where(command =>
                string.Equals(command.Parameter(), usedCommandParameter.Value(), StringComparison.OrdinalIgnoreCase)
                || string.Equals(command.Alias(), usedCommandParameter.Value(), StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (command != null)
            {
                //TODO: Use return bool.
                command.Execute(filename.Value());
            }
            else
            {
                Logging.Print("Command not found.", Logging.PrintLevel.Warning);
            }
        }
        catch (FileNotFoundException)
        {
            Logging.Print($"File {filename.Value()} not found.", Logging.PrintLevel.Error);
        }
        catch (UnauthorizedAccessException)
        {
            Logging.Print($"Failed to access {filename.Value()}", Logging.PrintLevel.Error);
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
