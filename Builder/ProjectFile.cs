namespace CopperGameTools.Builder;

public class ProjectFile
{
    public FileInfo SourceFile { get; }
    public List<ProjectFileKey> FileKeys { get; }

    public ProjectFile(FileInfo sourceFile)
    {
        if (!sourceFile.Exists)
        {
            SourceFile = new FileInfo("");
            FileKeys = [];
            Logging.Print($"The project file {sourceFile} does not exists / could not be found! Aborting.", Logging.PrintLevel.Error);
            return;
        }

        SourceFile = sourceFile;
        FileKeys = [];

        LoadKeysFromFile();
    }

    /// <summary>
    /// Rescans the Project file and Reads all valid Keys.
    /// </summary>
    private void LoadKeysFromFile()
    {
        foreach (string line in File.ReadAllLines(SourceFile.FullName))
        {
            if (!line.Contains('=') || line.StartsWith('#') || !IsValidString(line)) continue;
            string[] split = line.Split("=");
            FileKeys.Add(new ProjectFileKey(split[0], split[1]));
        }
    }

    public string GetKey(string searchKey)
    {
        if (!IsValidString(searchKey)) return ProjectFileKeys.InvalidKey;
        foreach (ProjectFileKey key in FileKeys)
        {
            if (key.Key != searchKey) continue;
            if (!key.Value.Contains('$')) return key.Value;
            string[] split = key.Value.Split('$', StringSplitOptions.RemoveEmptyEntries);
            foreach (string t in split)
            {
                string temp = GetKey(t);
                if (string.IsNullOrEmpty(temp)) continue;
                key.Value = temp;
            }
            return key.Value;
        }
        return "";
    }

    public bool GetKeyAsBoolean(string searchKey, bool defaultValue)
    {
        string key = GetKey(searchKey);
        return key != "" ? Convert.ToBoolean(key) : defaultValue;
    }

    private static bool IsValidString(string searchKey)
    {
        return !string.IsNullOrEmpty(searchKey) && !string.IsNullOrWhiteSpace(searchKey);
    }

    /// <summary>
    /// Initiates a Check of the Project file.
    /// </summary>
    /// <returns>A ProjectFileCheckResult. You can take a look at the ProjectFileCheckResult class to get an idea.</returns>
    /// <see cref="ProjectBuilderResultType"/>
    public ProjectFileCheckResult CheckProjectFile()
    {
        var errors = new List<ProjectFileCheckError>();
        int lineNumber = 1;

        var readKeys = new List<ProjectFileKey>();
        foreach (string line in File.ReadAllLines(SourceFile.FullName))
        {
            if (!IsValidString(line) || line.StartsWith($"#"))
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
                keySplit[1]);

            foreach (ProjectFileKey key in readKeys)
            {
                if (key.Key != keyToAdd.Key) continue;
                errors.Add(
                    new ProjectFileCheckError(ProjectFileCheckErrorType.DuplicatedKey, $"[{lineNumber}] {line}"));
                lineNumber++;
            }

            readKeys.Add(keyToAdd);
            lineNumber++;
        }

        return errors.Count > 0 ? new ProjectFileCheckResult(CgtProjectFileCheckResultType.Errors, errors)
            : new ProjectFileCheckResult(CgtProjectFileCheckResultType.NoErrors, []);
    }
}

