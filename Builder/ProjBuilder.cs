using CopperGameTools.Builder;

namespace CopperGameTools.Builder
{
    public class ProjBuilder
    {
        public ProjFile ProjFile { get; }

        public ProjBuilder(ProjFile cgtProjFile)
        {
            ProjFile = cgtProjFile;
        }

        /// <summary>
        /// Builds the Project defined by the .PKF Projectfile.
        /// </summary>
        /// <returns>Returns a ProjBuilderResult. The result may vary in case of errors.</returns>
        public ProjBuilderResult Build()
        {
            Console.WriteLine($"Building with CopperGameTools v{Utils.GetVersion()}:");

            if (ProjFile == null || ProjFile.SourceFile.DirectoryName == null)
                return new ProjBuilderResult(ProjBuilderResultType.FailedNoErrors);

            ProjFileCheckResult check = ProjFile.FileCheck();
            foreach (var error in check.ResultErrors)
                if (error.IsCritical) // exit with errors when a critical error is found
                    return new ProjBuilderResult(ProjBuilderResultType.FailedWithErrors);

            string projectName = ProjFile.KeyGet("project.name");
            string sourceDir = Path.Combine(ProjFile.SourceFile.DirectoryName, ProjFile.KeyGet("project.src.dir"));
            string sourceOut = ProjFile.KeyGet("project.src.out");
            string outDir = Path.Combine(ProjFile.SourceFile.DirectoryName, ProjFile.KeyGet("project.out.dir"));
            string mainFile = sourceDir + ProjFile.KeyGet("project.src.main");

            if (sourceDir == null || outDir == null)
                return new ProjBuilderResult(ProjBuilderResultType.FailedWithErrors);

            // Step 1: Packing Code Files

            Console.WriteLine($"STEP 1: Packing JavaScript Code into {sourceOut}.js...");

            string toWrite = $"// Generated using CopperGameTools v{Utils.GetVersion()} //\n";

            // write keys from pkf file
            foreach (ProjFileKey key in ProjFile.FileKeys)
            {
                toWrite += $"ccbSetCopperCubeVariable('{key.Key}','{key.Value}');\n";
            }

            List<string> listedSourceFiles = (Directory.GetFiles(sourceDir, "*.js", SearchOption.AllDirectories)).ToList();

            // write source files (except main)
            foreach (var file in listedSourceFiles)
            {
                if (file == mainFile) continue;
                FileInfo info = new(file);
                toWrite += $"// -- {info.Name.ToUpper()} -- //" +
                    $"\n{File.ReadAllText(file)}\n";
            }

            // write main file
            toWrite += File.ReadAllText(mainFile);

            // add main call with args
            toWrite += "Main(ccbGetCopperCubeVariable('project.src.args').split(' '));";

            toWrite = toWrite.Replace("\n\n", "\n");

            // create .js file with all the code
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
            File.WriteAllText(outDir + sourceOut + ".js", toWrite);
            Console.WriteLine("Done!\n");

            // Step 2: External resources

            Console.WriteLine("STEP 2: Process External Resources:");
            string folder = ProjFile.KeyGet("project.externalres.dir");
            if (folder != "" && Directory.Exists(folder))
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                ContentPacker.ContentPacker.Pack(Path.GetFullPath(folder), projectName.ToLower(), ProjFile.KeyGet("project.externalres.out"));
                Console.WriteLine("Done!\n");
            }
            Console.WriteLine($"Packed Source: {sourceOut + ".js"}\n" +
                              $"Number of Errors: {check.ResultErrors.Count()}");
            return check.ResultErrors.Count() > 0 ? new ProjBuilderResult(ProjBuilderResultType.DoneWithErrors) :
                new ProjBuilderResult(ProjBuilderResultType.DoneNoErrors);
        }
    }

    /// <summary>
    /// Class-Wrapper for ProjBuilderResultTypes.
    /// </summary>
    public class ProjBuilderResult
    {
        public ProjBuilderResult(ProjBuilderResultType cgtProjBuilderResultType)
        {
            ResultType = cgtProjBuilderResultType;
        }

        public ProjBuilderResultType ResultType { get; }
    }

    /// <summary>
    /// Types of a ProjBuilderResult.
    /// </summary>
    public enum ProjBuilderResultType
    {
        DoneNoErrors,
        DoneWithErrors,
        FailedNoErrors,
        FailedWithErrors
    }

}
