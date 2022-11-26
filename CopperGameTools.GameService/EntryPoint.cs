using System.Diagnostics;

namespace CopperGameTools.GameService;

class EntryPoint {
    public static void Main(String[] args) {
        if (args.Length == 0) return;
        var gameProcess = new Process();
        gameProcess.StartInfo.FileName = args[0];
        gameProcess.StartInfo.UseShellExecute = false;
        gameProcess.StartInfo.RedirectStandardOutput = true;

        using StreamReader reader = gameProcess.StandardOutput;

        gameProcess.Start();

        System.Console.WriteLine($"Starting Game Service for {gameProcess.StartInfo.FileName}");

        // loop while the game is running
        while (!gameProcess.HasExited) {
            Thread.Sleep(1000);
        }

        System.Console.WriteLine($"Ending Game Service for {gameProcess.StartInfo.FileName}");
    }
}