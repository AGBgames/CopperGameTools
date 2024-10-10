namespace CopperGameTools.CGT.Commands
{
    internal interface ICommand
    {
        /// <summary>
        /// Parameter of the command (e.g. "build" or "check).
        /// </summary>
        /// <returns>Command parameter string.</returns>
        string Parameter();

        /// <summary>
        /// Alias of the command parameter (e.g. for "build" it would be "b").
        /// </summary>
        /// <returns>Command parameter alias string.</returns>
        string Alias();

        /// <summary>
        /// Method defining what the command does.
        /// </summary>
        /// <param name="filename">Name of the loaded .cgt-File.</param>
        /// <returns>A boolean indecating success or failure.</returns>
        bool Execute(string filename);
    }
}
