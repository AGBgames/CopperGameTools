using System;
using System.Windows.Input;

namespace CopperGameTools.CopperUI;

public class CGTActionCommand : ICommand
{
    private readonly Action _action;

    public CGTActionCommand(Action action)
    {
        _action = action;
    }

    public void Execute(object parameter)
    {
        _action();
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public event EventHandler CanExecuteChanged;
}