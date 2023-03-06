using System.IO.Compression;

namespace CopperGameTools.ContentPacker
{
    public class ContentPacker
    {
        /*
         * Packs the files off the given folder into a .cgc file.
         */
        public static void Pack(string path, string name, string outDir)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            // delete old file
            if (File.Exists(outDir + name + ".cgc"))
                File.Delete(outDir + name + ".cgc");

            string[] content_files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            string[] supportedFileFormats = { "txt", "png", "bmp" };

            ZipArchive zip = ZipFile.Open(outDir + name + ".cgc", ZipArchiveMode.Create);
            foreach (var content_file in content_files)
            {
                string fileFormat = content_file.Split(".")[content_file.Split(".").Length - 1];

                if (!supportedFileFormats.Contains(fileFormat))
                {
                    Console.WriteLine($"Fileformat of {content_file} is not supported!");
                    continue;
                }

                var name_current_file = Path.GetFileName(content_file);
                zip.CreateEntryFromFile(content_file, name_current_file);
                Console.WriteLine("Written " + name_current_file + " to cgc.");
            }
        }

        /*
         * Unpacks a .cgc file and puts its files into a Data folder
         */
        public static void Unpack(string contentFile)
        {
            if (!Directory.Exists("Data")) { Directory.CreateDirectory("Data"); }

            if (!File.Exists(contentFile))
            {
                Console.WriteLine("Content file does not exist!");
                return;
            }

            ZipArchive zip = ZipFile.Open(contentFile + ".cgc", ZipArchiveMode.Read);
            Console.WriteLine("Unpacking content to directory...");
            zip.ExtractToDirectory("Data");
            Console.WriteLine("Done!");
        }

        /*
         * Deletes the Data folder.
         */
        public static void Clean()
        {
            Console.WriteLine("Cleaning up content-directory...");
            if (Directory.Exists("Data")) { Directory.Delete("Data", true); }
            Console.WriteLine("Done!");
        }
    }

}
