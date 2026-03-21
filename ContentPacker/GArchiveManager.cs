using System.IO.Compression;

namespace ContentPacker;

public class GArchiveManager
{
    public void CreateSimpleArchive(string outputPath, string[] filesToPack)
    {
        using var zip = new ZipArchive(new FileStream(outputPath, FileMode.Create), ZipArchiveMode.Update);

        string resourceType = "resource";
        
        foreach (string file in filesToPack)
        {
            if (file.EndsWith(".meta"))
            {
                resourceType = File.ReadAllText(file);
                continue;
            }
            string entryName = Path.GetRelativePath("Project/Resource/", file);
            zip.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
        }

        File.WriteAllText(outputPath + ".meta", resourceType);
    }

    public void ExtractSimpleArchive(string outputPath, string inputPath)
    {
        using var zip = new ZipArchive(new FileStream(inputPath, FileMode.Open), ZipArchiveMode.Read);
        foreach (var entry in zip.Entries)
        {
            string fullPath = Path.GetFullPath(Path.Combine(outputPath, entry.FullName));

            // Sicherheitscheck: Verhindert "Zip Slip" (Dateien außerhalb des Zielordners)
            if (!fullPath.StartsWith(Path.GetFullPath(outputPath), StringComparison.OrdinalIgnoreCase))
            {
                throw new IOException("Ungültiger Pfad im Archiv entdeckt!");
            }

            // DER ENTSCHEIDENDE SCHRITT:
            // Den Verzeichnisnamen extrahieren (z.B. Resource\core\textures)
            string? directoryName = Path.GetDirectoryName(fullPath);
            
            if (!string.IsNullOrEmpty(directoryName))
            {
                // Erstellt den gesamten Pfadbaum, falls er noch nicht da ist
                Directory.CreateDirectory(directoryName);
            }

            // Jetzt kann die Datei sicher extrahiert werden
            // Prüfen, ob es kein reiner Ordner-Eintrag ist
            if (!string.IsNullOrEmpty(entry.Name)) 
            {
                entry.ExtractToFile(fullPath, overwrite: true);
            }
        }
    }
}
