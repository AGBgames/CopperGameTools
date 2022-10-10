namespace CopperGameTools.Builder;

public class CGTProjFile
{
    private readonly FileInfo _sourceFile;

    public CGTProjFile(FileInfo sourceFile)
    {
        _sourceFile = sourceFile;
    }
    
    public List<CGTProjFileKey> KeysToList()
    {
        return File.ReadAllLines(_sourceFile.FullName)
        .Select(line => new CGTProjFileKey(line.Split('=')[0]
        .Replace(" ", ""), line.Split("=")[1].Replace(" ", "")))
        .ToList();
    }
    
    public void Test()
    {
        //TODO: Sheesh!
    }

    public string KeyFromList(string searchKey, List<CGTProjFileKey> keys)
    {
        foreach (var key in keys)
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
