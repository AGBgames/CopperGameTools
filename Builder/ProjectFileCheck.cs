﻿namespace CopperGameTools.Builder;

/// <summary>
/// Class-Wrapper for ProjectFileCheckResult
/// </summary>
/// <seealso cref="ProjectFileCheckError"/>
public class ProjectFileCheckResult(CgtProjectFileCheckResultType resultType, List<ProjectFileCheckError> resultErrors)
{
    public CgtProjectFileCheckResultType ResultType { get; } = resultType;
    public List<ProjectFileCheckError> ResultErrors { get; } = resultErrors;
    public bool FoundErrors => ResultErrors.Count > 0;
}

public enum CgtProjectFileCheckResultType
{
    NoErrors,
    Errors
}

/// <summary>
/// Class-Wrapper for ProjectFileCheckErrorType.
/// </summary>
/// <see cref="ProjectFileCheckErrorType"/>
public class ProjectFileCheckError(ProjectFileCheckErrorType errorType, string errorText)
{
    public ProjectFileCheckErrorType ErrorType { get; } = errorType;
    public string ErrorText { get; } = errorText;
}

/// <summary>
/// All Types of Project file Checking Errors.
/// </summary>
public enum ProjectFileCheckErrorType
{
    InvalidKey,
    InvalidValue,
    InvalidComment,
    DuplicatedKey
}

