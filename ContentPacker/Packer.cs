using System.IO.Compression;

namespace CopperGameTools.ContentPacker;

public abstract class Packer
{
    /// <summary>
    /// Packs all the files from the project.externalres.dir-folder into a ZIP like file.
    /// </summary>
    public static void Pack(string contentSourcePath, string outputFileName, string outputDirectory)
    {
        string zipFileName = outputDirectory + outputFileName;

        if (!Directory.Exists(contentSourcePath))
        {
            Directory.CreateDirectory(outputDirectory);
            Console.WriteLine("Source directory for external resources not does not exist. \n" +
                "The folder was created, but no files will be added to the .cgc file.");
            return;
        }
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        // delete old file
        if (File.Exists(zipFileName + ".cgc"))
            File.Delete(zipFileName + ".cgc");

        string[] contentFiles = Directory.GetFiles(contentSourcePath, "*.*", SearchOption.AllDirectories);

        ZipArchive zipArchive = ZipFile.Open(zipFileName + ".cgc", ZipArchiveMode.Create);
        foreach (string contentFile in contentFiles)
        {
            string nameOfCurrentFile = Path.GetFileName(contentFile);
            zipArchive.CreateEntryFromFile(contentFile, nameOfCurrentFile);
            Console.WriteLine("Written " + nameOfCurrentFile + " to cgc.");
        }
    }

    /// <summary>
    /// Unpacks the packed files into the Data folder.
    /// </summary>
    public static void Unpack(string contentFile, string unpackFolder = "Data")
    {
        if (!Directory.Exists(unpackFolder)) { Directory.CreateDirectory(unpackFolder); }

        if (!File.Exists(contentFile))
        {
            Console.WriteLine("Content file does not exist!");
            return;
        }

        ZipArchive zipArchive = ZipFile.Open(contentFile + ".cgc", ZipArchiveMode.Read);
        Console.WriteLine("Unpacking content to directory...");
        zipArchive.ExtractToDirectory(unpackFolder);
        Console.WriteLine("Done!");
    }

    /// <summary>
    /// Deletes up the Data folder.
    /// </summary>
    public static void Clean()
    {
        Console.WriteLine("Cleaning up content-directory...");
        if (Directory.Exists("Data"))
            Directory.Delete("Data", true);
        Console.WriteLine("Done!");
    }
}

