using System;
using System.Windows;
using System.Windows.Input;

namespace TodoList.WPF.Infrastructure.Commands;

public class DragMoveCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => (parameter as Window)?.DragMove();
}
