using System.Diagnostics;
using CopperGameTools.ContentPacker;

namespace CopperGameTools.GameLauncher;

class EntryPoint
{
    public static void Main(String[] args)
    {
        if (args.Length == 0) return;

        string gameName = args[0];
        string packFileName = args[1];

        // Unpack content
        Console.WriteLine($"Unpacking content from {packFileName}...");
        Packer.Unpack(packFileName);

        Console.WriteLine($"Launching {gameName}...");
        // handle game process
        Process gameProcess = OpenGameProcess(gameName, out StreamReader reader);

        // loop while the game is running
        while (!gameProcess.HasExited)
        {
            Thread.Sleep(1800);
            string? command = reader.ReadLine();
            if (command == null || command.Equals("")) continue;
            string[] commandArgs = command.Split(" ");

            switch (command)
            {
                case "exit":
                case "quit":
                    gameProcess.Kill();
                    break;
                case "export":
                    Packer.Unpack(packFileName, "Export");
                    Console.WriteLine("Exported Data to Export/");
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

        Packer.Clean();
        Console.WriteLine($"Ended!");
    }

    private static Process OpenGameProcess(string gameName, out StreamReader reader)
    {
        Process gameProcess = new();
        gameProcess.StartInfo.FileName = gameName;
        gameProcess.StartInfo.UseShellExecute = false;
        gameProcess.StartInfo.RedirectStandardOutput = true;

        gameProcess.Start();

        reader = gameProcess.StandardOutput;

        Console.WriteLine($"Started!");

        return gameProcess;
    }
}
