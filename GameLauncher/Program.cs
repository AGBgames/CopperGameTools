// See https://aka.ms/new-console-template for more information

public class Program
{
    private static readonly string GARCHIVE_LIST = "gpacklist.txt";
    private static readonly string GARCHIVE_INDEX_LIST = "gpackindex.txt";

    public static void Main(string[] args)
    {
        List<string> loaded = LoadGameArchives();
        Console.WriteLine("Loaded {0} archives!", loaded.Count);
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
}