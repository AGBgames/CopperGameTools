using System.Diagnostics;
using CopperGameTools.Shared;

namespace CopperGameTools.Builder;

public class ProjectBuilder(ProjectFile cgtProjectFile)
{
    public ProjectFile ProjectFile { get; } = cgtProjectFile;

    /// <summary>
    /// Builds the Project defined by the .PKF Project file.
    /// </summary>
    /// <returns>Returns a ProjectBuilderResult.</returns>
    public ProjectBuilderResult Build()
    {
        Console.WriteLine($"Building with CopperGameTools v{Constants.Version}");

        if (ProjectFile.SourceFile.DirectoryName == null)
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithErrors,
            "ProjectFile path is null.");

        ProjectFileCheckResult checkResult = ProjectFile.CheckProjectFile();
        if (checkResult.FoundErrors)
        {
            Console.WriteLine($"{checkResult.ResultErrors.Count} Error(s) have been found! Aborting...\n");
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectFile contains errors.");
        }

        if (ProjectFile.GetKey("builder.version") != Constants.Version)
        {
            Console.WriteLine("Project file set for different version of CopperCubeGameTools.\nSupport might be limited.");
        }

        string projectName = ProjectFile.GetKey("project.name");
        if (projectName == "")
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectFile:project.name is not valid.");

        string sourceDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey("project.src.dir"));
        if (!Directory.Exists(sourceDir))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithErrors,
                "SourcePath-Dir not found.");

        string sourceOut = ProjectFile.GetKey("project.src.out");
        if (sourceOut == "")
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectFile:project.src.out is not valid.");

        string outDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey("project.out.dir"));
        if (!Directory.Exists(outDir))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectOut-Dir not found.");

        string mainFileName = ProjectFile.GetKey("project.src.main");
        if (mainFileName == "")
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "ProjectFile:project.src.main is not valid.");
        if (!File.Exists(sourceDir + mainFileName))
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithErrors,
                "MainSourceFile not found.");

        // Step 1: Packing Code Files

        Console.WriteLine($"STEP 1: Packing JavaScript Code into {sourceOut}.js...");

        string toPutInOutputFile = $"// Generated using CopperGameTools v{Constants.Version} //\n";
        toPutInOutputFile += $"//Made for CopperCube Engine v{Constants.SupportedCopperCubeVersion}. //\n";

        foreach (ProjectFileKey key in ProjectFile.FileKeys)
        {
            toPutInOutputFile += $"ccbSetCopperCubeVariable('{key.Key}','{key.Value}');\n";
        }

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
            Console.WriteLine($"Wrote {info.Name} to packed source file.");
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
            Console.WriteLine("Failed to write file!");
            return ProjectBuilderResult.Of(ProjectBuilderResultType.FailedWithProjectFileErrors,
                "SourceOut-File failed to write.");
        }
        Console.WriteLine($"Packed Source to {outDir + sourceOut + ".js"}\n");

        // Step 2: External resources

        // Step 2: Custom Post-Build Commands
        Console.WriteLine("STEP 2: Custom Post-Build Commands:");
        if (ProjectFile.GetKeyAsBoolean("builder.commands.enabled"))
        {
            PostBuildCommand();
        }

        Console.WriteLine($"Done!");

        return ProjectBuilderResult.Of(ProjectBuilderResultType.DoneNoErrors, "Done.");
    }

    private void PostBuildCommand()
    {
        try
        {
            string value = ProjectFile.GetKey("builder.commands.postbuild");
            if (value == "none") return;

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
            Console.WriteLine(e.Message);
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
