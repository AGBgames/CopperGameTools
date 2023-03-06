using System.IO.Compression;

namespace CopperGameTools.ContentPacker;
public class ContentPacker
{
	public static void Pack(string path, string name)
	{
        if (File.Exists(name + ".cgc")) File.Delete(name + ".cgc");
        string[] content_files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        using ZipArchive zip = ZipFile.Open(name + ".cgc", ZipArchiveMode.Create);
        foreach (var content_file in content_files)
        {
            var name_current_file = Path.GetFileName(content_file);
            zip.CreateEntryFromFile(content_file, name_current_file);
            Console.WriteLine("Written " + name_current_file + " to cgc.");
        }
    }

	public static void Unpack(string contentFile)
    {
        Directory.CreateDirectory("~content");

        using ZipArchive zip = ZipFile.Open(contentFile + ".cgc", ZipArchiveMode.Read);
        Console.WriteLine("Unpacking content to directory...");
        zip.ExtractToDirectory("~content");
        Console.WriteLine("Done!");
    }

	public static void Clean()
	{
        Console.WriteLine("Cleaning up content-directory...");
		Directory.Delete("~content", true);
        Console.WriteLine("Done!");
	}
}
