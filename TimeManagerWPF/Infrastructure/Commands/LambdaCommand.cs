using System;
using System.Windows.Input;

namespace TodoList.WPF;
public abstract class Command : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public virtual bool CanExecute(object parameter) => true;

    public abstract void Execute(object parameter);
}

public class LambdaCommand : Command
{
    private readonly Action<object> action;
    private readonly Func<object, bool> canExecute;
    private string? login;
    private Func<object, bool> p;
    private ICommand? loaded;

    public LambdaCommand(ICommand? loaded)
    {
        this.loaded = loaded;
    }

    public LambdaCommand(Action<object> action, Func<object, bool> canExecute = null)
    {
        this.action = action ?? throw new ArgumentNullException(nameof(action));
        this.canExecute = canExecute;
    }

    public LambdaCommand(string? login, Func<object, bool> p)
    {
        this.login = login;
        this.p = p;
    }

    public override bool CanExecute(object parameter)
    {
        return canExecute?.Invoke(parameter) ?? true;
    }

    public override void Execute(object parameter)
    {
        action(parameter);
    }
}
