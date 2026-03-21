// See https://aka.ms/new-console-template for more information

using System.IO.Compression;

namespace ContentPacker;

using System.IO;

public static class ContentPacker
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
            return;
        
        string action = args[0];
        string folderName = args[1];
        if (!Directory.Exists(folderName))
            return;
        
        GArchiveManager gArchiveManager = new GArchiveManager();

        switch (action)
        {
            case "ma":
            {
                gArchiveManager.CreateSimpleArchive(folderName + ".garchive", Directory.GetFiles(folderName, "*.*",  SearchOption.AllDirectories));
                break;
            }
            case "ea":
            {
                if (args.Length < 3)
                    return;
                string inputPath = args[2];
                
                gArchiveManager.ExtractSimpleArchive(folderName, inputPath);
                break;
            }
        }
        
    }
}
