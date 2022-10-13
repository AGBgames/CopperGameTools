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

        foreach (var line in File.ReadAllLines(SourceFile.FullName))
        {
            if (!line.Contains("=") || line.StartsWith("#")) continue;
            FileKeys.Add(new CGTProjFileKey(line.Split('=')[0], line.Split('=')[1]));
        }
    }

    public void ReloadKeys()
    {
        if (!SourceFile.Exists)
            throw new IOException("Source File does not exist!");

        FileKeys.Clear();
        FileKeys = new List<CGTProjFileKey>();

        foreach (var line in File.ReadAllLines(SourceFile.FullName))
        {
            if (!line.Contains("=") || line.StartsWith("#") || string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;
            FileKeys.Add(new CGTProjFileKey(line.Split('=')[0], line.Split('=')[1]));
        }
    }

    public string KeyGet(string searchKey)
    {
        foreach (var key in FileKeys)
        {
            if (key.Key == searchKey) return key.Value;
        }
        return "";
    }

    // Checks the file for errors (invalid comments and keys etc)
    public CGTProjFileCheckResult FileCheck()
    {
        var errors = new List<CGTProjFileCheckError>();
        var lineNumber = 1;
        var criticalKeys = new[] {"src", "projectname", "out"};

        foreach (var line in File.ReadAllLines(SourceFile.FullName))
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
                lineNumber++;
                continue;
            }
            
            if (!line.Contains('=') && !line.StartsWith("#"))
            {
                var isCritic = criticalKeys.Contains(line) && line.StartsWith(criticalKeys.Any().ToString());
                errors.Add(new CGTProjFileCheckError(CGTProjFileCheckErrorType.InvalidKey, isCritic, $"[{lineNumber}] '{line}'"));
            }

            lineNumber++;
        }

        return errors.Count > 0 ? new CGTProjFileCheckResult(CGTProjFileCheckResultType.Errors, errors) : new CGTProjFileCheckResult(CGTProjFileCheckResultType.NoErrors, new List<CGTProjFileCheckError>());
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
    InvalidComment
}

#endregion File Check Error