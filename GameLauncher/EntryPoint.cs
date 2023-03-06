using System.Diagnostics;

namespace CopperGameTools.GameLauncher;

class EntryPoint {
    public static void Main(String[] args) {
        if (args.Length == 0) return;

        // Unpack content
        ContentPacker.ContentPacker.Unpack(args[1]);
        Console.WriteLine($"Unpacking content from {args[1]}...");

        Console.WriteLine($"Launching {args[0]}...");
        // handle game process
        Process gameProcess = new Process();
        gameProcess.StartInfo.FileName = args[0];
        gameProcess.StartInfo.UseShellExecute = false;
        gameProcess.StartInfo.RedirectStandardOutput = true;

        gameProcess.Start();

        using StreamReader reader = gameProcess.StandardOutput;

        Console.WriteLine($"Started!");

        // loop while the game is running
        while (!gameProcess.HasExited) {
            Thread.Sleep(800);
        }

        ContentPacker.ContentPacker.Clean();
        Console.WriteLine($"Ended!");
    }
}
