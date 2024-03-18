namespace CopperGameTools.Builder
{
    public class ProjectFile
    {
        public FileInfo SourceFile { get; }
        public List<ProjectFileKey> FileKeys { get; set; }

        public ProjectFile(FileInfo sourceFile)
        {
            if (!sourceFile.Exists || sourceFile == null)
            {
                SourceFile = new FileInfo("");
                FileKeys = new List<ProjectFileKey>();
                return;
            }

            SourceFile = sourceFile;
            FileKeys = new List<ProjectFileKey>();

            LoadKeysFromFile();
        }

        public void RefreshKeysFromFile()
        {
            try
            {
                FileKeys.Clear();
                LoadKeysFromFile();
            }
            catch (Exception)
            {
                Console.WriteLine("Error while reloading keys!");
            }
        }

        /// <summary>
        /// Rescans the Projectfile and Reads all valid Keys.
        /// </summary>
        private void LoadKeysFromFile()
        {
            var lineNumber = 1;
            foreach (var line in File.ReadAllLines(SourceFile.FullName))
            {
                if (!line.Contains("=") || line.StartsWith("#") || string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;
                string[] split = line.Split("=");
                FileKeys.Add(new ProjectFileKey(split[0], split[1], lineNumber));
                lineNumber++;
            }
        }

        public string GetKey(string searchKey)
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
                        if (string.IsNullOrEmpty(GetKey(split[i]))) continue;
                        key.Value.Replace($"${split[i]}$", GetKey(split[i]));
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
        public string GetKey(int line)
        {
            foreach (var key in FileKeys)
            {
                if (key.Line == line) return key.Value;
            }
            return "";
        }

        /// <summary>
        /// Initiates a Check of the Projectfile.
        /// </summary>
        /// <returns>A ProjFileCheckResult. You can take a look at the ProjFileCheckResult class to get an idea.</returns>
        /// <see cref="ProjectBuilderResultType"/>
        public ProjFileCheckResult CheckProjectFile()
        {
            var errors = new List<ProjFileCheckError>();
            var lineNumber = 1;

            var readKeys = new List<ProjectFileKey>();
            foreach (var line in File.ReadAllLines(SourceFile.FullName))
            {
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    lineNumber++;
                    continue;
                }

                if (!line.Contains('='))
                {
                    errors.Add(new ProjFileCheckError(ProjFileCheckErrorType.InvalidKey, $"[{lineNumber}] {line}"));
                    lineNumber++;
                    continue;
                }

                if (line.Split('=')[1] == "")
                {
                    errors.Add(new ProjFileCheckError(ProjFileCheckErrorType.InvalidValue, $"[{lineNumber}] {line}"));
                    lineNumber++;
                    continue;
                }

                string[] keySplit = line.Split('=');
                var keyToAdd = new ProjectFileKey(
                    keySplit[0],
                    keySplit[1],
                    lineNumber);

                foreach (var key in readKeys)
                {
                    if (key.Key == keyToAdd.Key)
                    {
                        errors.Add(
                            new ProjFileCheckError(ProjFileCheckErrorType.DuplicatedKey, $"[{lineNumber}] {line}"));
                        lineNumber++;
                        continue;
                    }
                }

                readKeys.Add(keyToAdd);
                lineNumber++;
            }

            return errors.Count > 0 ? new ProjFileCheckResult(CGTProjFileCheckResultType.Errors, errors) 
                : new ProjFileCheckResult(CGTProjFileCheckResultType.NoErrors, new List<ProjFileCheckError>());
        }

        // private static bool IsKeyCritical(string line, string[] criticalKeys)
        // {
        //     bool isCritic = false;
        //     if (criticalKeys.Contains(line.Replace("=", "")))
        //     {
        //         foreach (var criticKey in criticalKeys)
        //         {
        //             if (line.StartsWith(criticKey)) 
        //                 isCritic = true;
        //         }
        //     }

        //     return isCritic;
        // }
    }

    #region File Check

    /// <summary>
    /// Class-Wrapper for ProjFileCheckResult
    /// </summary>
    /// <seealso cref="ProjFileCheckError"/>
    public class ProjFileCheckResult(CGTProjFileCheckResultType resultType, List<ProjFileCheckError> resultErrors)
    {
        public CGTProjFileCheckResultType ResultType { get; } = resultType;
        public List<ProjFileCheckError> ResultErrors { get; } = resultErrors;
        public bool FoundErrors => ResultErrors.Count > 0 ? true : false;
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
        public ProjFileCheckError(ProjFileCheckErrorType errorType, string errorText)
        {
            ErrorType = errorType;
            ErrorText = errorText;
        }

        public ProjFileCheckErrorType ErrorType { get; }
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

