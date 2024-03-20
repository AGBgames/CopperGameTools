using System.Diagnostics;

namespace CopperGameTools.GameLauncher;

class EntryPoint {
    public static void Main(String[] args) {
        if (args.Length == 0) return;

        string gameName = args[0];
        string packFileName = args[1];

        // Unpack content
        Console.WriteLine($"Unpacking content from {packFileName}...");
        ContentPacker.Packer.Unpack(packFileName);

        Console.WriteLine($"Launching {gameName}...");
        // handle game process
        Process gameProcess = new();
        gameProcess.StartInfo.FileName = gameName;
        gameProcess.StartInfo.UseShellExecute = false;
        gameProcess.StartInfo.RedirectStandardOutput = true;

        gameProcess.Start();

        using StreamReader reader = gameProcess.StandardOutput;

        Console.WriteLine($"Started!");

        // loop while the game is running
        while (!gameProcess.HasExited) {
            Thread.Sleep(1800);
            string? command = reader.ReadLine();
            if (command == null || command.Equals("")) continue;
            string[] commandArgs = command.Split(" ");

            if (command.Equals("exit") || command.Equals("quit"))
            {
                gameProcess.Kill();
                break;
            }
            else if (command.Equals("export"))
            {
                ContentPacker.Packer.Unpack(packFileName, "Export");
                Console.WriteLine("Exported Data to Export/");
            }
            else
            {
                Console.WriteLine("Unknown command.");
            }
        }

        ContentPacker.Packer.Clean();
        Console.WriteLine($"Ended!");
    }
}
