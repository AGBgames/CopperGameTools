using System.IO.Compression;

namespace CopperGameTools.ContentPacker
{
    public class ContentPacker
    {
        /// <summary>
        /// Packs all the files from the project.externalres.dir-folder into a ZIP like file.
        /// </summary>
        public static void Pack(string path, string name, string outDir)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            // delete old file
            if (File.Exists(outDir + name + ".cgc"))
                File.Delete(outDir + name + ".cgc");

            string[] contentFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            string[] supportedFileFormats = { "txt", "png", "bmp", "jgp", "wav", "ogg" };

            ZipArchive zip = ZipFile.Open(outDir + name + ".cgc", ZipArchiveMode.Create);
            foreach (var contentFile in contentFiles)
            {
                string fileFormat = contentFile.Split(".")[contentFile.Split(".").Length - 1];

                if (!supportedFileFormats.Contains(fileFormat))
                {
                    Console.WriteLine($"Fileformat of {contentFile} is not supported, skipping...");
                    continue;
                }

                var nameOfCurrentFile = Path.GetFileName(contentFile);
                zip.CreateEntryFromFile(contentFile, nameOfCurrentFile);
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

            ZipArchive zip = ZipFile.Open(contentFile + ".cgc", ZipArchiveMode.Read);
            Console.WriteLine("Unpacking content to directory...");
            zip.ExtractToDirectory(unpackFolder);
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

}
