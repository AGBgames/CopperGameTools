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
    public ProjectBuilderResultType Build()
    {
        Console.WriteLine($"Building with CopperGameTools v{Constants.Version}");

        if (ProjectFile.SourceFile.DirectoryName == null)
            return ProjectBuilderResultType.FailedWithErrors;

        ProjectFileCheckResult checkResult = ProjectFile.CheckProjectFile();
        if (checkResult.FoundErrors)
        {
            Console.WriteLine($"{checkResult.ResultErrors.Count} Error(s) have been found! Aborting...\n");
            return ProjectBuilderResultType.FailedWithProjectFileErrors;
        }

        if (ProjectFile.GetKey("builder.version") != Constants.Version)
        {
            Console.WriteLine("Project file set for different version of CopperCubeGameTools.\nSupport might be limited.");
        }

        string projectName = ProjectFile.GetKey("project.name");
        if (projectName == "")
            return ProjectBuilderResultType.FailedWithProjectFileErrors;

        string sourceDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey("project.src.dir"));
        if (!Directory.Exists(sourceDir))
            return ProjectBuilderResultType.FailedWithErrors;

        string sourceOut = ProjectFile.GetKey("project.src.out");
        if (sourceOut == "")
            return ProjectBuilderResultType.FailedWithProjectFileErrors;

        string outDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey("project.out.dir"));
        if (!Directory.Exists(outDir))
            return ProjectBuilderResultType.FailedWithErrors;

        string mainFileName = ProjectFile.GetKey("project.src.main");
        if (mainFileName == "")
            return ProjectBuilderResultType.FailedWithProjectFileErrors;
        if (!File.Exists(sourceDir + mainFileName))
            return ProjectBuilderResultType.FailedWithErrors;

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
            += File.ReadAllText(mainFile);

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
            return ProjectBuilderResultType.FailedWithErrors;
        }
        Console.WriteLine($"Packed Source to {outDir + sourceOut + ".js"}\n");

        // Step 2: External resources

        // Step 2: Custom Post-Build Commands
        Console.WriteLine("STEP 3: Custom Post-Build Commands:");
        if (ProjectFile.GetKeyAsBoolean("builder.commands.enabled"))
        {
            PostBuildCommand();
        }

        Console.WriteLine($"Done!");

        return ProjectBuilderResultType.DoneNoErrors;
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

/// <summary>
/// Types of a ProjectBuilderResult.
/// </summary>
public enum ProjectBuilderResultType
{
    DoneNoErrors,
    FailedWithErrors,
    FailedWithProjectFileErrors
}
