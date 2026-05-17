namespace Dv.App.Interfaces;

using System;
using System.Threading.Tasks;

public interface IDialogService
{
    Task ShowValidationDialogAsync(string message);
    
    Task<bool> ShowConfirmationDialogAsync(
        string period,
        string boilerId,
        DateTime startDateTime,
        DateTime endDateTime,
        DateTime periodStart,
        DateTime periodEnd);
}