using CopperGameTools.Shared;

namespace CopperGameTools.Builder;

public class ProjectFile
{
    public FileInfo SourceFile { get; }
    public List<ProjectFileKey> FileKeys { get; }

    public ProjectFile(FileInfo sourceFile)
    {
        if (!sourceFile.Exists)
        {
            SourceFile = new FileInfo("error");
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
            if (!Utils.IsValidString(line) || !line.Contains('=') || line.StartsWith('#')) continue;
            string[] split = line.Split("=");
            FileKeys.Add(new ProjectFileKey(split[0], split[1]));
        }
    }

    /// <summary>
    /// Looks for the key with the specified name and returns its value if found.
    /// </summary>
    /// <param name="searchKey">Name of the key to search for.</param>
    /// <returns>Value of key.</returns>
    public string GetKey(string searchKey)
    {
        if (!Utils.IsValidString(searchKey)) 
            return ProjectFileKeys.InvalidKey;
        ProjectFileKey? keyFound = 
            FileKeys.Find(x => x.Key == searchKey);
        if (keyFound == null)
            return ProjectFileKeys.InvalidKey;
        if (!keyFound.Value.Contains('$'))
            return keyFound.Value;
        string[] split = keyFound.Value.Split('$', StringSplitOptions.RemoveEmptyEntries);
        string tempValue = GetKey(split[0]);
        if (!Utils.IsValidString(tempValue)) 
            return ProjectFileKeys.InvalidKey;
        keyFound.Value = tempValue;
        return keyFound.Value;
    }

    /// <summary>
    /// Same as GetKey(), but converts value to int.
    /// </summary>
    /// <param name="searchKey">Name of the key to search for.</param>
    /// <param name="defaultValue">The default value to return if key is not found.</param>
    /// <returns>Value of key as an int or the default value.</returns>
    public int GetKeyAsInt(string searchKey, int defaultValue = 0)
    {
        string key = GetKey(searchKey);
        return Utils.IsValidString(key) ? Convert.ToInt32(key) : defaultValue;
    }

    /// <summary>
    /// Same as GetKey(), but converts value to boolean.
    /// </summary>
    /// <param name="searchKey">Name of the key to search for.</param>
    /// <param name="defaultValue">The default value to return if key is not found.</param>
    /// <returns>Value of key as an boolean or the default value.</returns>
    public bool GetKeyAsBoolean(string searchKey, bool defaultValue = false)
    {
        string key = GetKey(searchKey);
        return Utils.IsValidString(key) ? Convert.ToBoolean(key) : defaultValue;
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
            if (!Utils.IsValidString(line) || line.StartsWith('#'))
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

            if (!Utils.IsValidString(line.Split('=')[1]))
            {
                errors.Add(new ProjectFileCheckError(ProjectFileCheckErrorType.InvalidValue, $"[{lineNumber}] {line}"));
                lineNumber++;
                continue;
            }

            string[] keySplit = line.Split('=');
            var keyToAdd = new ProjectFileKey(keySplit[0], keySplit[1]);
            
            foreach (ProjectFileKey unused in readKeys.Where(key => key.Key == keyToAdd.Key))
            {
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

