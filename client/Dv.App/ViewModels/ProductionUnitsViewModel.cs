using System.Collections.ObjectModel;
using Dv.App.Models;

namespace Dv.App.ViewModels;

// Gets data from MaintenanceStore.cs to make it visible in UI
public sealed class ProductionUnitsViewModel : ViewModelBase
{
    public ObservableCollection<MaintenanceEvent> MaintenanceSchedules => MaintenanceStore.MaintenanceSchedules;
}
