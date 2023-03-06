using System.IO.Compression;

namespace CopperGameTools.ContentPacker;
public class ContentPacker
{
	public static void Pack(string path, string name)
	{
        if (File.Exists(name + ".cgc")) File.Delete(name + ".cgc");
		var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        using ZipArchive zip = ZipFile.Open(name + ".cgc", ZipArchiveMode.Create);
        foreach (var item in files)
        {
            var nameg = Path.GetFileName(item);
            zip.CreateEntryFromFile(item, nameg);
            Console.WriteLine("Written " + nameg + " to cgc.");
        }
    }

	public static void Unpack(string contentFile)
    {
        Directory.CreateDirectory("~content");

        using ZipArchive zip = ZipFile.Open(contentFile + ".cgc", ZipArchiveMode.Read);
        System.Console.WriteLine("Unpacking content to directory...");
        zip.ExtractToDirectory("~content");
        System.Console.WriteLine("Done!");
    }

	public static void Clean()
	{
        System.Console.WriteLine("Cleaning up content-directory...");
		Directory.Delete("~content", true);
        System.Console.WriteLine("Done!");
	}
}
