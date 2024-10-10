namespace CopperGameTools.CGT.Commands
{
    internal interface ICommand
    {
        string Parameter();
        string Alias();
        bool Execute(string filename);
    }
}
