namespace CopperGameTools.Builder;

public abstract class ProjectFileKeys
{
    /// <summary>
    /// Invalid key
    /// </summary>
    public const string InvalidKey = "";
    
    /// <summary>
    /// Version of CGT the project should be used with.
    /// </summary>
    public const string BuilderVersion = "builder.version";
    /// <summary>
    /// Enforce the version needed, set by builder.version.
    /// </summary>
    public const string BuilderRequireVersion = "builder.require_version";
    
    /// <summary>
    /// Name of the project.
    /// </summary>
    public const string ProjectName = "project.name";
    /// <summary>
    /// Directory of the javascript source files.
    /// </summary>
    public const string ProjectSourceDirectory = "project.src.dir";
    /// <summary>
    /// Name of the javascript output file (should be same as project.name)
    /// </summary>
    public const string ProjectOutputFilename = "project.src.out";
    /// <summary>
    /// Name of the main javascript file inside the source directory.
    /// </summary>
    public const string ProjectSourceMainFilename = "project.src.main";

    /// <summary>
    /// Name of the output directory (usually where the CopperCube-Project-File is located)
    /// </summary>
    public const string ProjectOutputDirectory = "project.out.dir";
}