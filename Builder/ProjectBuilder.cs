using System.Diagnostics;
using CopperGameTools.Shared;

namespace CopperGameTools.Builder;

public class ProjectBuilder(ProjectFile cgtProjectFile)
{
    public ProjectFile ProjectFile { get; } = cgtProjectFile;

    /// <summary>
    /// Builds the Project defined by the .CGT Project file.
    /// </summary>
    /// <returns>Returns a ProjectBuilderResult.</returns>
    public ProjectBuilderResult Build()
    {
        DateTime start = DateTime.Now;
        
        Logging.Print($"Building with CopperGameTools v{CopperGameToolsInfo.Version}", Logging.PrintLevel.Info);

        string version = ProjectFile.GetKey(ProjectFileKeys.BuilderVersion);
        bool versionRequired = ProjectFile.GetKeyAsBoolean(ProjectFileKeys.BuilderRequireVersion);

        if (!version.Equals(ProjectFileKeys.InvalidKey))
        {
            switch (version != CopperGameToolsInfo.Version && version != CopperGameToolsInfo.MajorVersion)
            {
                case true when !versionRequired:
                    Logging.Print("Project file set for different version of CopperGameTools.\nSupport might be limited.\n",
                        Logging.PrintLevel.Warning);
                    break;
                case true when versionRequired:
                    Logging.Print("Unable to build due to incompatible versions.\nPlease refer to the project file for more information.",
                        Logging.PrintLevel.Error);
                    return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors);
            }
        }
        
        if (ProjectFile.SourceFile.DirectoryName == null)
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithErrors);

        ProjectFileCheckResult checkResult = ProjectFile.CheckProjectFile();
        if (checkResult.FoundErrors)
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors);

        string projectName = ProjectFile.GetKey(ProjectFileKeys.ProjectName);
        if (projectName.Equals(ProjectFileKeys.InvalidKey))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors);

        string sourceDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey(ProjectFileKeys.ProjectSourceDirectory));
        if (!Directory.Exists(sourceDir))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithErrors);

        string sourceOut = ProjectFile.GetKey(ProjectFileKeys.ProjectOutputFilename);
        if (sourceOut.Equals(ProjectFileKeys.InvalidKey))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors);

        string outDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey(ProjectFileKeys.ProjectOutputDirectory));
        if (!Directory.Exists(outDir))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors);

        string mainFileName = ProjectFile.GetKey(ProjectFileKeys.ProjectSourceMainFilename);
        if (mainFileName.Equals(ProjectFileKeys.InvalidKey))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors);
        if (!File.Exists(sourceDir + mainFileName))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithErrors);

        // Step 1: Packing Code Files

        Logging.Print($"STEP 1: Packing Code into {sourceOut}:",
            Logging.PrintLevel.Info);

        string toPutInOutputFile = $"// Generated using CopperGameTools v{CopperGameToolsInfo.Version} //\n";
        toPutInOutputFile += $"//Made for CopperCube Engine v{CopperGameToolsInfo.SupportedCopperCubeVersion}. //\n";

        foreach (ProjectFileKey key in ProjectFile.FileKeys)
            toPutInOutputFile += $"ccbSetCopperCubeVariable('{key.Key}','{key.Value}');\n";

        string mainFile = sourceDir + mainFileName;
        string mainFileExtension = new FileInfo(mainFile).Extension;

        Logging.Print("Looking for " + mainFileExtension + "files in " + sourceDir, Logging.PrintLevel.Info);
        
        List<string> sourceFileList
            = [.. Directory.GetFiles(sourceDir, "*" + mainFileExtension, SearchOption.AllDirectories)];
        sourceFileList.Remove(mainFile);

        foreach (string file in sourceFileList)
        {
            FileInfo info = new(file);
            toPutInOutputFile += $"// -- {info.Name.ToUpper()} -- //" +
                $"\n{File.ReadAllText(file)}\n";
            Logging.Print($"Added {info.Name} to output.", Logging.PrintLevel.Info);
        }

        toPutInOutputFile
            += File.ReadAllText(mainFile);

        // add main call with args
        toPutInOutputFile
            += "\n" + "Main(ccbGetCopperCubeVariable('project.src.args').split(' '));";

        toPutInOutputFile = toPutInOutputFile.Replace("\n\n", "\n");

        // create .js file with all the code
        try
        {
            File.WriteAllText(outDir + sourceOut + mainFileExtension, toPutInOutputFile);
            Logging.Print($"Wrote Packed Source to {outDir + sourceOut + "." + mainFileExtension}\n", Logging.PrintLevel.Info);
        }
        catch (Exception)
        {
            Logging.Print("Failed to write file!", Logging.PrintLevel.Error);
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors);
        }

        // Step 2: Custom Post-Build Commands
        Logging.Print($"STEP 2: Running post-build commands:", Logging.PrintLevel.Info);
        if (ProjectFile.GetKeyAsBoolean("builder.commands.enabled"))
            PostBuildCommand();
        
        DateTime end = DateTime.Now;
        TimeSpan duration = end - start;

        Logging.Print($"Done in {duration.Milliseconds}ms!", Logging.PrintLevel.Info);

        return ProjectBuilderResult.Of(ProjectBuilderResultType.DoneNoErrors);
    }
    
    private void PostBuildCommand()
    {
        try
        {
            var command = new StrongReadOnlyHolder<string>(ProjectFile.GetKey("builder.commands.postbuild"));

            var startInfo = new ProcessStartInfo
            {
                FileName = command.Value(),
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }
        catch (Exception e)
        {
            Logging.Print(e.Message, Logging.PrintLevel.Error);
        }
    }
}

public class ProjectBuilderResult
{
    public ProjectBuilderResultType ResultType { get; private set; }

    public static ProjectBuilderResult Of(ProjectBuilderResultType resultType)
    {
        return new ProjectBuilderResult()
        {
            ResultType = resultType,
        };
    }
}

/// <summary>
/// Types of a ProjectBuilderResult.
/// </summary>
public enum ProjectBuilderResultType
{
    DoneNoErrors,
    FailedWithErrors,
    FailedWithProjectFileErrors
}
