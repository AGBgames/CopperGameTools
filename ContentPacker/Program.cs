// See https://aka.ms/new-console-template for more information

using System.IO.Compression;

namespace ContentPacker;

using System.IO;
using System.Text;

public class ContentPacker
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
            return;
        
        string action = args[0];
        string folderName = args[1];
        

        if (!Directory.Exists(folderName))
            return;
        
        CreatePackage(folderName + ".gpack", Directory.GetFiles(folderName, "*.*",  SearchOption.AllDirectories));
        CreateSimpleArchive(folderName + ".garchive", Directory.GetFiles(folderName, "*.*",  SearchOption.AllDirectories));
    }
    
    public static void CreatePackage(string outputPath, string[] filesToPack)
    {
        using (var fs = new FileStream(outputPath, FileMode.Create))
        using (var writer = new BinaryWriter(fs, Encoding.UTF8))
        {
            // 1. Header schreiben
            writer.Write("GPACK"u8.ToArray()); // Magic Number
            writer.Write((short)1); // Dateiformat Version
            writer.Write((short)100); // Engine Version (1.0.0)

            // 2. Platzhalter für TOC (Table of Contents).
            // Wir müssen später wissen, wie viele Dateien drin sind
            writer.Write(filesToPack.Length);

            long tocPosition = fs.Position;
            // Wir reservieren Platz für Offsets (später wichtig zum Springen)
            // Pro Datei: Name (String), Offset (Long), Size (Int).
            // Einfachheitshalber schreiben wir die Daten erst und merken uns die Positionen.

            // 3. Daten schreiben & Metadaten sammeln
            var fileEntries = new List<(string name, long offset, int size)>();

            // Wir springen über den TOC-Bereich hinaus und schreiben erst die Daten
            fs.Seek(tocPosition + (filesToPack.Length * 256), SeekOrigin.Begin);

            foreach (var file in filesToPack)
            {
                byte[] data = File.ReadAllBytes(file);
                long offset = fs.Position;
                writer.Write(data);
                fileEntries.Add((Path.GetFileName(file), offset, data.Length));
            }

            // 4. TOC zurückschreiben
            fs.Seek(tocPosition, SeekOrigin.Begin);
            foreach (var entry in fileEntries)
            {
                writer.Write(entry.name.PadRight(100)); // Fixe Länge für einfachen Zugriff
                writer.Write(entry.offset);
                writer.Write(entry.size);
            }
        }
    }

    public static void CreateSimpleArchive(string outputPath, string[] filesToPack)
    {
        using var zip = new ZipArchive(new FileStream(outputPath, FileMode.Create), ZipArchiveMode.Update);
        foreach (var file in filesToPack)
            zip.CreateEntryFromFile(file, file,  CompressionLevel.Optimal);
    }

    public static void ExtractSimpleArchive(string outputPath, string inputPath)
    {
        using var zip = new ZipArchive(new FileStream(inputPath, FileMode.Open), ZipArchiveMode.Update);
        foreach (var entry in zip.Entries)
            entry.ExtractToFile(outputPath, true);
    }
}