namespace CopperGameTools.Builder;

public class ProjectFile
{
    public FileInfo SourceFile { get; }
    public List<ProjectFileKey> FileKeys { get; set; }

    public ProjectFile(FileInfo sourceFile)
    {
        if (!sourceFile.Exists || sourceFile == null)
        {
            SourceFile = new FileInfo("");
            FileKeys = [];
            Console.WriteLine($"The project file {sourceFile} does not exists / could not be found! Aborting.");
            return;
        }

        SourceFile = sourceFile;
        FileKeys = [];

        LoadKeysFromFile();
    }

    public void RefreshKeysFromFile()
    {
        try
        {
            FileKeys.Clear();
            LoadKeysFromFile();
        }
        catch (Exception)
        {
            Console.WriteLine("Error while reloading keys!");
        }
    }

    /// <summary>
    /// Rescans the Project file and Reads all valid Keys.
    /// </summary>
    private void LoadKeysFromFile()
    {
        int lineNumber = 1;
        foreach (var line in File.ReadAllLines(SourceFile.FullName))
        {
            if (!line.Contains('=') || line.StartsWith('#') || string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;
            string[] split = line.Split("=");
            FileKeys.Add(new ProjectFileKey(split[0], split[1], lineNumber));
            lineNumber++;
        }
    }

    public string GetKey(string searchKey)
    {
        if (searchKey is null) return "";
        foreach (ProjectFileKey key in FileKeys)
        {
            if (key.Key != searchKey) continue;
            if (key.Value.Contains('$'))
            {
                var split = key.Value.Split('$', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < split.Length; i++)
                {
                    if (string.IsNullOrEmpty(GetKey(split[i]))) continue;
                    key.Value = GetKey(split[i]);
                }
            }
            return key.Value;
        }
        return "";
    }

    public int GetKeyAsInt(string searchKey)
    {
        string key = GetKey(searchKey);
        return key == "" ? 0 : Convert.ToInt32(key);
    }
    
    public double GetKeyAsDouble(string searchKey)
    {
        string key = GetKey(searchKey);
        return key == "" ? 0.0 : Convert.ToDouble(key);
    }

    public bool GetKeyAsBoolean(string searchKey)
    {
        string key = GetKey(searchKey);
        return key != "" && Convert.ToBoolean(key);
    }

    /// <summary>
    /// Gets a Key From a Specific Line.
    /// </summary>
    /// <param name="line">Line to Get The Key From.</param>
    /// <returns></returns>
    public string GetKey(int line)
    {
        foreach (var key in FileKeys)
        {
            if (key.Line == line) return key.Value;
        }
        return "";
    }

    /// <summary>
    /// Initiates a Check of the Project file.
    /// </summary>
    /// <returns>A ProjectFileCheckResult. You can take a look at the ProjectFileCheckResult class to get an idea.</returns>
    /// <see cref="ProjectBuilderResultType"/>
    public ProjectFileCheckResult CheckProjectFile()
    {
        var errors = new List<ProjectFileCheckError>();
        var lineNumber = 1;

        var readKeys = new List<ProjectFileKey>();
        foreach (var line in File.ReadAllLines(SourceFile.FullName))
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                lineNumber++;
                continue;
            }

            if (!line.Contains('='))
            {
                errors.Add(new ProjectFileCheckError(ProjectFileCheckErrorType.InvalidKey, $"[{lineNumber}] {line}"));
                lineNumber++;
                continue;
            }

            if (line.Split('=')[1] == "")
            {
                errors.Add(new ProjectFileCheckError(ProjectFileCheckErrorType.InvalidValue, $"[{lineNumber}] {line}"));
                lineNumber++;
                continue;
            }

            string[] keySplit = line.Split('=');
            var keyToAdd = new ProjectFileKey(
                keySplit[0],
                keySplit[1],
                lineNumber);

            foreach (var key in readKeys)
            {
                if (key.Key == keyToAdd.Key)
                {
                    errors.Add(
                        new ProjectFileCheckError(ProjectFileCheckErrorType.DuplicatedKey, $"[{lineNumber}] {line}"));
                    lineNumber++;
                    continue;
                }
            }

            readKeys.Add(keyToAdd);
            lineNumber++;
        }

        return errors.Count > 0 ? new ProjectFileCheckResult(CGTProjectFileCheckResultType.Errors, errors)
            : new ProjectFileCheckResult(CGTProjectFileCheckResultType.NoErrors, new List<ProjectFileCheckError>());
    }
}

