namespace CopperGameTools.Builder
{
    public class ProjectBuilder(ProjectFile cgtProjFile)
    {
        public ProjectFile ProjFile { get; } = cgtProjFile;
        
        /// <summary>
        /// Builds the Project defined by the .PKF Projectfile.
        /// </summary>
        /// <returns>Returns a ProjBuilderResult.</returns>
        public ProjBuilderResult Build()
        {
            Console.WriteLine($"Building with CopperGameTools v{Utils.GetVersion()}:");

            if (ProjFile == null || ProjFile.SourceFile.DirectoryName == null)
                return new ProjBuilderResult(ProjBuilderResultType.FailedNoErrors);

            ProjFileCheckResult check = ProjFile.CheckProjectFile();
            if (check.FoundErrors)
            {
                Console.WriteLine($"{check.ResultErrors.Count} Error(s) have been found! Aborting...\n");
                return new ProjBuilderResult(ProjBuilderResultType.FailedWithErrors);
            }

            string projectName = ProjFile.GetKey("project.name");
            if (projectName == "")
                return new ProjBuilderResult(ProjBuilderResultType.FailedWithErrors);

            string sourceDir = Path.Combine(ProjFile.SourceFile.DirectoryName, ProjFile.GetKey("project.src.dir"));
            if (sourceDir == null || !Directory.Exists(sourceDir))
                return new ProjBuilderResult(ProjBuilderResultType.FailedWithErrors);

            string sourceOut = ProjFile.GetKey("project.src.out");
            if (sourceOut == "")
                return new ProjBuilderResult(ProjBuilderResultType.FailedWithErrors);

            string outDir = Path.Combine(ProjFile.SourceFile.DirectoryName, ProjFile.GetKey("project.out.dir"));
            if (outDir == null || !Directory.Exists(outDir))
                return new ProjBuilderResult(ProjBuilderResultType.FailedWithErrors);

            string mainFileName = ProjFile.GetKey("project.src.main");
            if (mainFileName == "" || !File.Exists(sourceDir + mainFileName))
                return new ProjBuilderResult(ProjBuilderResultType.FailedWithErrors);

            // Step 1: Packing Code Files

            Console.WriteLine($"STEP 1: Packing JavaScript Code into {sourceOut}.js...");

            string toPutInOutputFile = $"// Generated using CopperGameTools v{Utils.GetVersion()} //\n";
            toPutInOutputFile += "//Please keep CGT updated so functionality with newer versions of CopperCube is ensured. //\n";

            // add all keys in the pkf as coppercube variables to use during runtime.
            foreach (ProjectFileKey key in ProjFile.FileKeys)
            {
                toPutInOutputFile += $"ccbSetCopperCubeVariable('{key.Key}','{key.Value}');\n";
            }

            List<string> listedSourceFiles 
                = [.. (Directory.GetFiles(sourceDir, "*.js", SearchOption.AllDirectories))];

            // write source files (except main)
            foreach (var file in listedSourceFiles)
            {
                if (file == sourceDir + mainFileName) 
                    continue;
                FileInfo info = new(file);
                toPutInOutputFile += $"// -- {info.Name.ToUpper()} -- //" +
                    $"\n{File.ReadAllText(file)}\n";
            }

            // write main file
            toPutInOutputFile 
                += File.ReadAllText(sourceDir + mainFileName);

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
                return new ProjBuilderResult(ProjBuilderResultType.FailedNoErrors);
            }
            Console.WriteLine("Done!");

            // Step 2: External resources

            Console.WriteLine("STEP 2: Process External Resources:");
            string externalResourcesFolder = ProjFile.GetKey("project.externalres.dir");
            if (externalResourcesFolder != "")
            {
                if (!Directory.Exists(externalResourcesFolder))
                    Directory.CreateDirectory(externalResourcesFolder);
                ContentPacker.ContentPacker.Pack(Path.GetFullPath(externalResourcesFolder), projectName.ToLower(), ProjFile.GetKey("project.externalres.out"));
                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("No External Resources Folder defined, no resources will be packed!");
            }

            Console.WriteLine($"Packed Source to {sourceOut + ".js"}");

            return new ProjBuilderResult(ProjBuilderResultType.DoneNoErrors);
        }
    }

    /// <summary>
    /// Class-Wrapper for ProjBuilderResultTypes.
    /// </summary>
    public class ProjBuilderResult(ProjBuilderResultType cgtProjBuilderResultType)
    {
        public ProjBuilderResultType ResultType { get; } = cgtProjBuilderResultType;
    }

    /// <summary>
    /// Types of a ProjBuilderResult.
    /// </summary>
    public enum ProjBuilderResultType
    {
        DoneNoErrors,
        FailedNoErrors,
        FailedWithErrors
    }

}
