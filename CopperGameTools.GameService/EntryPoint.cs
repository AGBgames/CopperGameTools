using System.Diagnostics;

namespace CopperGameTools.GameService;

class EntryPoint {
    public static void Main(String[] args) {
        if (args.Length == 0) return;

        System.Console.WriteLine("GameService v0.1.2");

        // Unpack content
        ContentPacker.ContentPacker.Unpack(args[1]);

        // handle game process
        var gameProcess = new Process();
        gameProcess.StartInfo.FileName = args[0];
        gameProcess.StartInfo.UseShellExecute = false;
        gameProcess.StartInfo.RedirectStandardOutput = true;

        gameProcess.Start();

        using StreamReader reader = gameProcess.StandardOutput;

        System.Console.WriteLine($"Starting Game Service for {gameProcess.StartInfo.FileName}");

        // loop while the game is running
        while (!gameProcess.HasExited) {
            Thread.Sleep(1000);
        }

        System.Console.WriteLine($"Ending Game Service for {gameProcess.StartInfo.FileName}");
        ContentPacker.ContentPacker.Clean();
    }
}
