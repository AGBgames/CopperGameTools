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
        Utils.Print($"Building with CopperGameTools v{Constants.Version}", Utils.PrintLevel.Info);

        string version = ProjectFile.GetKey("builder.version");
        bool versionRequired = ProjectFile.GetKeyAsBoolean("builder.require_version", false);

        if (version != Constants.Version && !versionRequired)
        {
            Utils.Print("Project file set for different version of CopperGameTools.\nSupport might be limited.\n",
                Utils.PrintLevel.Warn);
        }
        else if (version != Constants.Version && versionRequired)
        {
            Utils.Print("Unable to build due to incompatible versions.\nPlease refer to the project file for more information.",
                Utils.PrintLevel.Error);
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors, 
                "Incompatible version");
        }
        
        if (ProjectFile.SourceFile.DirectoryName == null)
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithErrors,
            "ProjectFile:path is null.");

        ProjectFileCheckResult checkResult = ProjectFile.CheckProjectFile();
        if (checkResult.FoundErrors)
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectFile contains errors.");

        string projectName = ProjectFile.GetKey("project.name");
        if (projectName.Equals(""))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectFile:project.name is not valid.");

        string sourceDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey("project.src.dir"));
        if (!Directory.Exists(sourceDir))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithErrors,
                "SourcePath-Dir not found.");

        string sourceOut = ProjectFile.GetKey("project.src.out");
        if (sourceOut.Equals(""))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectFile:project.src.out is not valid.");

        string outDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey("project.out.dir"));
        if (!Directory.Exists(outDir))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectOut-Dir not found.");

        string mainFileName = ProjectFile.GetKey("project.src.main");
        if (mainFileName.Equals(""))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectFile:project.src.main is not valid.");
        if (!File.Exists(sourceDir + mainFileName))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithErrors,
                "MainSourceFile not found.");

        // Step 1: Packing Code Files

        Utils.Print($"STEP 1: Packing JavaScript Code into {sourceOut}.js:",
            Utils.PrintLevel.Info);

        string toPutInOutputFile = $"// Generated using CopperGameTools v{Constants.Version} //\n";
        toPutInOutputFile += $"//Made for CopperCube Engine v{Constants.SupportedCopperCubeVersion}. //\n";

        foreach (ProjectFileKey key in ProjectFile.FileKeys)
            toPutInOutputFile += $"ccbSetCopperCubeVariable('{key.Key}','{key.Value}');\n";

        List<string> sourceFileList
            = [.. Directory.GetFiles(sourceDir, "*.js", SearchOption.AllDirectories)];
        string mainFile = sourceDir + mainFileName;

        foreach (string file in sourceFileList)
        {
            if (file.Equals(mainFile))
                continue;
            FileInfo info = new(file);
            toPutInOutputFile += $"// -- {info.Name.ToUpper()} -- //" +
                $"\n{File.ReadAllText(file)}\n";
            Utils.Print($"Wrote {info.Name} to packed source file.", Utils.PrintLevel.Info);
        }

        toPutInOutputFile
            += "\n" + File.ReadAllText(mainFile);

        // add main call with args
        toPutInOutputFile
            += "Main(ccbGetCopperCubeVariable('project.src.args').split(' '));";

        toPutInOutputFile = toPutInOutputFile.Replace("\n\n", "\n");

        // create .js file with all the code
        try
        {
            File.WriteAllText(outDir + sourceOut + ".js", toPutInOutputFile);
        }
        catch (Exception)
        {
            Utils.Print("Failed to write file!", Utils.PrintLevel.Error);
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "SourceOut-File failed to write.");
        }
        Utils.Print($"Wrote Packed Source to {outDir + sourceOut + ".js"}\n", Utils.PrintLevel.Info);

        // Step 2: External resources

        // Step 2: Custom Post-Build Commands
        Utils.Print($"STEP 2: Running post-build commands:", Utils.PrintLevel.Info);
        if (ProjectFile.GetKeyAsBoolean("builder.commands.enabled", false))
            PostBuildCommand();

        Utils.Print($"Done!", Utils.PrintLevel.Info);

        return ProjectBuilderResult.Of(ProjectBuilderResultType.DoneNoErrors, "Done.");
    }

    private void PostBuildCommand()
    {
        try
        {
            string value = ProjectFile.GetKey("builder.commands.postbuild");

            var startInfo = new ProcessStartInfo
            {
                FileName = value,
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
            Utils.Print(e.Message, Utils.PrintLevel.Error);
        }
    }
}

public class ProjectBuilderResult
{
    public ProjectBuilderResultType ResultType { get; private set; }
    public string ResultCauseInformation { get; private set; } = "";

    public static ProjectBuilderResult Of(ProjectBuilderResultType resultType, string resultCauseInformation)
    {
        return new ProjectBuilderResult()
        {
            ResultType = resultType,
            ResultCauseInformation = resultCauseInformation
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
