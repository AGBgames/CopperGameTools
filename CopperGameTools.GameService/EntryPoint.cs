using System.Diagnostics;

namespace CopperGameTools.GameService;

class EntryPoint {
    public static void Main(String[] args) {
        if (args.Length == 0) return;
        var gameProcess = new Process();
        gameProcess.StartInfo.FileName = args[0];

        gameProcess.Start();

        System.Console.WriteLine($"Starting Game Service for {gameProcess.StartInfo.FileName}");

        while (!gameProcess.HasExited) {
            Thread.Sleep(2000);
        }

        System.Console.WriteLine($"Ending Game Service for {gameProcess.StartInfo.FileName}");
    }
}