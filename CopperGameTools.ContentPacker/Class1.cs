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
        zip.ExtractToDirectory("~content");
    }

	public static void Clean()
	{
		Directory.Delete("~content", true);
	}
}
