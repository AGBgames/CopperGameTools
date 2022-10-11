using System.Collections;

namespace CopperGameTools.Builder;

public class CGTProjFile
{
    public FileInfo SourceFile { get; }
    public List<CGTProjFileKey> FileKeys { get; }

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
        return new CGTProjFileCheckResult(CGTProjFileCheckResultType.NoErrors, new CGTProjFileCheckError[] {});
    }
}

#region File Check

public class CGTProjFileCheckResult
{
    public CGTProjFileCheckResult(CGTProjFileCheckResultType resultType, CGTProjFileCheckError[] resultErrors)
    {
        ResultType = resultType;
        ResultErrors = resultErrors;
    }

    public CGTProjFileCheckResultType ResultType { get; }
    public CGTProjFileCheckError[] ResultErrors { get; }
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
    public CGTProjFileCheckError(CGTProjFileCheckErrorType errorType, bool isCritical)
    {
        ErrorType = errorType;
        IsCritical = isCritical;
    }

    public CGTProjFileCheckErrorType ErrorType { get;  }
    public bool IsCritical { get; }
}

public enum CGTProjFileCheckErrorType
{
    InvalidKey,
    InvalidValue,
    InvalidComment
}

#endregion File Check Error