using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dv.App.Commands;

public sealed class AsyncCommand : ICommand
{
    private readonly Func<Task> execute;
    private readonly Func<bool>? canExecute;
    private bool isExecuting;

    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return !this.isExecuting && (this.canExecute?.Invoke() ?? true);
    }

    public async void Execute(object? parameter)
    {
        if (!this.CanExecute(parameter))
        {
            return;
        }

        this.isExecuting = true;
        this.RaiseCanExecuteChanged();

        try
        {
            await this.execute();
        }
        finally
        {
            this.isExecuting = false;
            this.RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}