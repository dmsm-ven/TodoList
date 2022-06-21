using System;
using System.Windows;
using System.Windows.Input;

namespace TodoList.WPF.Infrastructure.Commands;

public class RestoreWindowCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) 
    {
        Window window = (parameter as Window);

        if (window.WindowState == WindowState.Maximized)
            window.WindowState = WindowState.Normal;
        else
            window.WindowState = WindowState.Maximized;
    }
}
