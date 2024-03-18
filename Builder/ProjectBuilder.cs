namespace CopperGameTools.Builder
{
    public class ProjectBuilder(ProjectFile cgtProjFile)
    {
        public ProjectFile ProjectFile { get; } = cgtProjFile;
        
        /// <summary>
        /// Builds the Project defined by the .PKF Projectfile.
        /// </summary>
        /// <returns>Returns a ProjectBuilderResult.</returns>
        public ProjectBuilderResult Build()
        {
            Console.WriteLine($"Building with CopperGameTools v{Shared.Constants.Version}");

            if (ProjectFile.SourceFile.DirectoryName == null)
                return new ProjectBuilderResult(ProjectBuilderResultType.FailedNoErrors);

            ProjFileCheckResult check = ProjectFile.CheckProjectFile();
            if (check.FoundErrors)
            {
                Console.WriteLine($"{check.ResultErrors.Count} Error(s) have been found! Aborting...\n");
                return new ProjectBuilderResult(ProjectBuilderResultType.FailedWithErrors);
            }

            string projectName = ProjectFile.GetKey("project.name");
            if (projectName == "")
                return new ProjectBuilderResult(ProjectBuilderResultType.FailedWithErrors);

            string sourceDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey("project.src.dir"));
            if (sourceDir == null || !Directory.Exists(sourceDir))
                return new ProjectBuilderResult(ProjectBuilderResultType.FailedWithErrors);

            string sourceOut = ProjectFile.GetKey("project.src.out");
            if (sourceOut == "")
                return new ProjectBuilderResult(ProjectBuilderResultType.FailedWithErrors);

            string outDir = Path.Combine(ProjectFile.SourceFile.DirectoryName, ProjectFile.GetKey("project.out.dir"));
            if (outDir == null || !Directory.Exists(outDir))
                return new ProjectBuilderResult(ProjectBuilderResultType.FailedWithErrors);

            string mainFileName = ProjectFile.GetKey("project.src.main");
            if (mainFileName == "" || !File.Exists(sourceDir + mainFileName))
                return new ProjectBuilderResult(ProjectBuilderResultType.FailedWithErrors);

            // Step 1: Packing Code Files

            Console.WriteLine($"STEP 1: Packing JavaScript Code into {sourceOut}.js...");

            string toPutInOutputFile = $"// Generated using CopperGameTools v{Shared.Constants.Version} //\n";
            toPutInOutputFile += $"//Made for CopperCube Engine v{Shared.Constants.SupportedCopperCubeVersion}. //\n";

            // add all keys in the pkf as coppercube variables to use during runtime.
            foreach (ProjectFileKey key in ProjectFile.FileKeys)
            {
                toPutInOutputFile += $"ccbSetCopperCubeVariable('{key.Key}','{key.Value}');\n";
            }

            List<string> sourceFileList 
                = [.. (Directory.GetFiles(sourceDir, "*.js", SearchOption.AllDirectories))];
            string mainFile = sourceDir + mainFileName;

            foreach (var file in sourceFileList)
            {
                if (file == mainFile) 
                    continue;
                FileInfo info = new(file);
                toPutInOutputFile += $"// -- {info.Name.ToUpper()} -- //" +
                    $"\n{File.ReadAllText(file)}\n";
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
                return new ProjectBuilderResult(ProjectBuilderResultType.FailedNoErrors);
            }
            Console.WriteLine("Done!");

            // Step 2: External resources

            Console.WriteLine("STEP 2: Process External Resources:");
            string externalResourcesFolder = ProjectFile.GetKey("project.externalres.dir");
            if (externalResourcesFolder != "")
            {
                if (!Directory.Exists(externalResourcesFolder))
                    Directory.CreateDirectory(externalResourcesFolder);
                ContentPacker.ContentPacker.Pack(Path.GetFullPath(externalResourcesFolder), projectName.ToLower(), ProjectFile.GetKey("project.externalres.out"));
                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("No External Resources Folder defined, no resources will be packed!");
            }

            Console.WriteLine($"Packed Source to {sourceOut + ".js"}");

            return new ProjectBuilderResult(ProjectBuilderResultType.DoneNoErrors);
        }
    }

    /// <summary>
    /// Class-Wrapper for ProjBuilderResultTypes.
    /// </summary>
    public class ProjectBuilderResult(ProjectBuilderResultType cgtProjBuilderResultType)
    {
        public ProjectBuilderResultType ResultType { get; } = cgtProjBuilderResultType;
    }

    /// <summary>
    /// Types of a ProjectBuilderResult.
    /// </summary>
    public enum ProjectBuilderResultType
    {
        // Done without any errors generated by the project file.
        DoneNoErrors,
        // Failed without any errors generated by the project file.
        FailedNoErrors,
        // Failed with errors generated by the project file.
        FailedWithErrors
    }

}
