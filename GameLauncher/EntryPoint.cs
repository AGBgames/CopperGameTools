using System.Diagnostics;

namespace CopperGameTools.GameLauncher;

class EntryPoint {
    public static void Main(String[] args) {
        if (args.Length == 0) return;

        string gameName = args[0];
        string packFile = args[1];

        // Unpack content
        Console.WriteLine($"Unpacking content from {packFile}...");
        ContentPacker.ContentPacker.Unpack(packFile);

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
            string? command = Console.ReadLine();
            if (command == null) continue;
            var commandArgs = command.Split(" ");
            if (command == "quit")
            {
                gameProcess.Kill();
                break;
            }
            else if (command.StartsWith("export "))
            {
                if (commandArgs.Length < 2) continue;
                ContentPacker.ContentPacker.Unpack(packFile, "Export");
                Console.WriteLine("Exported Data to Export/");
            }
        }

        ContentPacker.ContentPacker.Clean();
        Console.WriteLine($"Ended!");
    }
}
