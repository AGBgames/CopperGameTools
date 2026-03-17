// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;

public class Program
{
    private static readonly string GARCHIVE_LIST = "gpacklist.txt";
    private static readonly string GARCHIVE_INDEX_LIST = "gpackindex.txt";

    private static readonly string MODS_PATH = "TDR3/Mods/";

    private static List<string> gameArguments = [];

    public static void Main(string[] args)
    {
        List<string> loaded = LoadGameArchives();
        Console.WriteLine("Loaded {0} archives!", loaded.Count);

        if (args.Length == 0)
            return;

        string what = args[0];
        if (what == "legacy")
        {
            RunGame();
        }
        else if (what == "modded")
        {
            RunGameModded();
        }
    }

    private static List<string> LoadGameArchives()
    {
        List<string> loadedArchives = [];

        if (File.Exists(GARCHIVE_INDEX_LIST))
        {
            loadedArchives = File.ReadAllLines(GARCHIVE_INDEX_LIST).ToList();
        }
        
        string[] archives = Directory.GetFiles("./", "*.garchive");

        foreach (string archive in archives)
        {
            if (loadedArchives.Contains(archive))
            {
                Console.WriteLine("GArchive already contains '{0}', skipping.", archive);
                continue;
            }
            
            if (!Directory.Exists("Resource\\"))
                Directory.CreateDirectory("Resource\\");

            if (archive.Contains("_res_"))
            {
                ContentPacker.ContentPacker.ExtractSimpleArchive("Resource\\", archive);
            }
            else if (archive.Contains("_dlc_"))
            {
                
            }
            
            loadedArchives.Add(archive);
        }
        
        File.WriteAllLines(GARCHIVE_INDEX_LIST, loadedArchives);
        
        return loadedArchives;
    }

    private static void RunGame()
    {
        Process gameProcess = new Process();
        gameProcess.StartInfo.FileName = "TheDarkRooms3";
        foreach (string gameArgument in gameArguments)
        {
            gameProcess.StartInfo.ArgumentList.Add(gameArgument);
        }
        gameProcess.Start();
        gameProcess.WaitForExit();
    }

    private static void RunGameModded()
    {
        string modLog = "";
        string[] modFolders = Directory.GetDirectories(MODS_PATH, "*", SearchOption.TopDirectoryOnly);
        
        StringBuilder moddedScript = new StringBuilder(File.ReadAllText(Path.Combine(MODS_PATH, "TheDarkRooms3.js")));
        moddedScript.AppendLine("Mods.runningModLoader = true;");
        
        foreach (string modFolder in modFolders)
        {
            DirectoryInfo modDirectory = new DirectoryInfo(modFolder);
            string modName = modDirectory.Name;
            
            Console.WriteLine("Found mod {0}", modName);
            
            moddedScript.AppendLine($"// Mod Start {modName} //");

            moddedScript.AppendLine($"var {modName} = {{SettingsPath: '{modFolder}/Settings'}};");
            
            string modMain = $"{modFolder}/Source/Main.js";
            moddedScript.AppendLine(File.ReadAllText(modMain));
            moddedScript.AppendLine($"{modName}Main();");
            
            moddedScript.AppendLine($"Mods.loadedMods.push('{modName}');");
            
            moddedScript.AppendLine($"// Mod End {modName} //");
        }
        
        File.WriteAllText(Path.Combine(MODS_PATH, "Modded.js"), moddedScript.ToString());
        
        gameArguments.Add("-script:");
        gameArguments.Add(Path.Combine(MODS_PATH, "Modded.js"));
        gameArguments.Add("-debug");
        
        RunGame();
    }
}