namespace CopperGameTools.Builder
{
    public class ProjFile
    {
        public FileInfo SourceFile { get; }
        public List<ProjFileKey> FileKeys { get; set; }
        public string[] CriticalKeys { get; set; }

        public ProjFile(FileInfo sourceFile)
        {
            if (!sourceFile.Exists || sourceFile == null)
            {
                SourceFile = new FileInfo("");
                FileKeys = new List<ProjFileKey>();
                CriticalKeys = new[] {""};
                Console.WriteLine("Failed to load file: File not found!");
                return;
            }

            SourceFile = sourceFile;
            FileKeys = new List<ProjFileKey>();

            // Add all keys that should cause an critical error (when not used properly)
            CriticalKeys = new[]
            {
            "project.name",
            "project.src.dir",
            "project.src.out",
            "project.out.dir",
            "project.src.main",
            "project.src.args",
            "project.externalres.dir",
            "project.externalres.out"
            };

            AddKeys();
        }

        public void ReloadKeys()
        {
            if (!SourceFile.Exists)
                throw new IOException("Source File does not exist!");

            FileKeys.Clear();
            FileKeys = new List<ProjFileKey>();

            AddKeys();
        }

        public string KeyGet(string searchKey)
        {
            if (searchKey == null) return "";
            foreach (var key in FileKeys)
            {
                if (key.Key != searchKey) continue;
                if (key.Value.Contains('$'))
                {
                    var split = key.Value.Split('$', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (string.IsNullOrEmpty(KeyGet(split[i]))) continue;
                        key.Value = key.Value.Replace($"${split[i]}$", KeyGet(split[i]));
                    }
                }
                return key.Value;
            }
            return "";
        }

        /// <summary>
        /// Gets a Key From a Specific Line.
        /// </summary>
        /// <param name="line">Line to Get The Key From.</param>
        /// <returns></returns>
        public string KeyGet(int line)
        {
            foreach (var key in FileKeys)
            {
                if (key.Line == line) return key.Value;
            }
            return "";
        }

        public void PrintErros()
        {
            foreach (var err in FileCheck().ResultErrors)
            {
                Console.WriteLine(
                    $"{err.ErrorText} | ErrorType -> {err.ErrorType} | Is Critical -> {err.IsCritical}"
                );
            }
        }

        /// <summary>
        /// Initiates a Check of the Projectfile.
        /// </summary>
        /// <returns>A ProjFileCheckResult. Result varies in case of errors.</returns>
        /// <see cref="ProjBuilderResultType"/>
        public ProjFileCheckResult FileCheck()
        {
            var errors = new List<ProjFileCheckError>();
            var lineNumber = 1;

            var readKeys = new List<ProjFileKey>();
            foreach (var line in File.ReadAllLines(SourceFile.FullName))
            {
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    lineNumber++;
                    continue;
                }

                if (!line.Contains('='))
                {
                    errors.Add(new ProjFileCheckError(ProjFileCheckErrorType.InvalidKey, IsCritic(line, CriticalKeys), $"[{lineNumber}] {line}"));
                    lineNumber++;
                    continue;
                }

                if (line.Split('=')[1] == "")
                {
                    errors.Add(new ProjFileCheckError(ProjFileCheckErrorType.InvalidValue, IsCritic(line, CriticalKeys), $"[{lineNumber}] {line}"));
                    lineNumber++;
                    continue;
                }

                var keyToAdd = new ProjFileKey(line.Split('=')[0],
                    line.Split('=')[1],
                    lineNumber);

                foreach (var key in readKeys)
                {
                    if (key.Key == keyToAdd.Key)
                    {
                        errors.Add(new ProjFileCheckError(ProjFileCheckErrorType.DuplicatedKey, IsCritic(line, CriticalKeys), $"[{lineNumber}] {line}"));
                        lineNumber++;
                        continue;
                    }
                }

                readKeys.Add(keyToAdd);
                lineNumber++;
            }

            return errors.Count > 0 ? new ProjFileCheckResult(CGTProjFileCheckResultType.Errors, errors) : new ProjFileCheckResult(CGTProjFileCheckResultType.NoErrors, new List<ProjFileCheckError>());
        }

        private bool IsCritic(string line, string[] criticalKeys)
        {
            var isCritic = false;
            if (criticalKeys.Contains(line.Replace("=", "")))
            {
                foreach (var criticKey in criticalKeys)
                {
                    if (line.StartsWith(criticKey)) isCritic = true;
                }
            }

            return isCritic;
        }

        /// <summary>
        /// Rescans the Projectfile and Reads all valid Keys.
        /// </summary>
        private void AddKeys()
        {
            var lineNumber = 1;
            foreach (var line in File.ReadAllLines(SourceFile.FullName))
            {
                if (!line.Contains("=") || line.StartsWith("#") || string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;
                FileKeys.Add(new ProjFileKey(line.Split('=')[0], line.Split('=')[1], lineNumber));
                lineNumber++;
            }
        }
    }

    #region File Check

    /// <summary>
    /// Class-Wrapper for ProjFileCheckResult
    /// </summary>
    /// <seealso cref="ProjFileCheckError"/>
    public class ProjFileCheckResult
    {
        public ProjFileCheckResult(CGTProjFileCheckResultType resultType, List<ProjFileCheckError> resultErrors)
        {
            ResultType = resultType;
            ResultErrors = resultErrors;
        }

        public CGTProjFileCheckResultType ResultType { get; }
        public List<ProjFileCheckError> ResultErrors { get; }
    }

    public enum CGTProjFileCheckResultType
    {
        NoErrors,
        Errors
    }

    #endregion File Check

    #region File Check Error

    /// <summary>
    /// Class-Wrappper for ProjFileCheckErrorType.
    /// </summary>
    /// <see cref="ProjFileCheckErrorType"/>
    public class ProjFileCheckError
    {
        public ProjFileCheckError(ProjFileCheckErrorType errorType, bool isCritical, string errorText)
        {
            ErrorType = errorType;
            IsCritical = isCritical;
            ErrorText = errorText;
        }

        public ProjFileCheckErrorType ErrorType { get; }
        public bool IsCritical { get; }
        public string ErrorText { get; }
    }

    /// <summary>
    /// All Types of Projectfile Checking Errors.
    /// </summary>
    public enum ProjFileCheckErrorType
    {
        InvalidKey,
        InvalidValue,
        InvalidComment,
        DuplicatedKey
    }

    #endregion File Check Error
}

