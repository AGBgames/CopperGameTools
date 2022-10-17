using System.Collections;
using System.Diagnostics;

namespace CopperGameTools.Builder;

public class CGTProjFile
{
    public FileInfo SourceFile { get; }
    public List<CGTProjFileKey> FileKeys { get; set; }

    public CGTProjFile(FileInfo sourceFile)
    {
        if (!sourceFile.Exists) 
            throw new IOException("Source File does not exist!");

        SourceFile = sourceFile;
        FileKeys = new List<CGTProjFileKey>();

        AddKeys();
    }

    public void ReloadKeys()
    {
        if (!SourceFile.Exists)
            throw new IOException("Source File does not exist!");

        FileKeys.Clear();
        FileKeys = new List<CGTProjFileKey>();

        AddKeys();
    }

    public string KeyGet(string searchKey)
    {
        if (searchKey == null) return "";
        foreach (var key in FileKeys)
        {
            if (key.Key == searchKey) return key.Value;
        }
        return "";
    }

    public string KeyGet(int line)
    {
        foreach (var key in FileKeys)
        {
            if (key.Line == line) return key.Value;
        }
        return "";
    }

    // Checks the file for errors (invalid comments and keys etc)
    public CGTProjFileCheckResult FileCheck()
    {
        var errors = new List<CGTProjFileCheckError>();
        var lineNumber = 1;
        var criticalKeys = new[]
        {
            "project.name", 
            "project.src.dir", 
            "project.out.dir", 
            "project.out.name",
            "project.out.version",
            "project.platform",
            "project.platform.cache",
            "project.platform.cache.path"
        };

        var readKeys = new List<CGTProjFileKey>();
        foreach (var line in File.ReadAllLines(SourceFile.FullName))
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                lineNumber++;
                continue;
            }
            
            if (!line.Contains('='))
            {
                errors.Add(new CGTProjFileCheckError(CGTProjFileCheckErrorType.InvalidKey, IsCritic(line, criticalKeys), $"[{lineNumber}] {line}"));
                lineNumber++;
                continue;
            }
            
            if (line.Split('=')[1] == "")
            {
                errors.Add(new CGTProjFileCheckError(CGTProjFileCheckErrorType.InvalidValue, IsCritic(line, criticalKeys), $"[{lineNumber}] {line}"));
                lineNumber++;
                continue;
            }
            
            var keyToAdd = new CGTProjFileKey(line.Split('=')[0], 
                line.Split('=')[1], 
                lineNumber);

            foreach (var key in readKeys)
            {
                if (key.Key == keyToAdd.Key)
                {
                    errors.Add(new CGTProjFileCheckError(CGTProjFileCheckErrorType.DuplicatedKey, IsCritic(line, criticalKeys), $"[{lineNumber}] {line}"));
                    lineNumber++;
                    continue;
                }
            }

            readKeys.Add(keyToAdd);
            lineNumber++;
        }

        return errors.Count > 0 ? new CGTProjFileCheckResult(CGTProjFileCheckResultType.Errors, errors) : new CGTProjFileCheckResult(CGTProjFileCheckResultType.NoErrors, new List<CGTProjFileCheckError>());
    }
    
    private bool IsCritic(string line, string[] criticalKeys)
    {
        var isCritic = false;
        if (criticalKeys.Contains(line.Replace("=", "")))
        {
            foreach (var criticKey in criticalKeys)
            {
                if (line.StartsWith(criticKey)) isCritic = true;
            }
        }

        return isCritic;
    }

    private void AddKeys()
    {
        var lineNumber = 1;
        foreach (var line in File.ReadAllLines(SourceFile.FullName))
        {
            if (!line.Contains("=") || line.StartsWith("#") || string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;
            FileKeys.Add(new CGTProjFileKey(line.Split('=')[0], line.Split('=')[1], lineNumber));
            lineNumber++;
        }
    }
}

#region File Check

public class CGTProjFileCheckResult
{
    public CGTProjFileCheckResult(CGTProjFileCheckResultType resultType, List<CGTProjFileCheckError> resultErrors)
    {
        ResultType = resultType;
        ResultErrors = resultErrors;
    }

    public CGTProjFileCheckResultType ResultType { get; }
    public List<CGTProjFileCheckError> ResultErrors { get; }
}

public enum CGTProjFileCheckResultType
{
    NoErrors,
    Errors
}

#endregion File Check

#region File Check Error

public class CGTProjFileCheckError
{
    public CGTProjFileCheckError(CGTProjFileCheckErrorType errorType, bool isCritical, string errorText)
    {
        ErrorType = errorType;
        IsCritical = isCritical;
        ErrorText = errorText;
    }

    public CGTProjFileCheckErrorType ErrorType { get;  }
    public bool IsCritical { get; }
    public string ErrorText { get; }
}

public enum CGTProjFileCheckErrorType
{
    InvalidKey,
    InvalidValue,
    InvalidComment,
    DuplicatedKey
}

#endregion File Check Error