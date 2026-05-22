namespace Dv.App.ViewModels;

using System.Collections.ObjectModel;
using Dv.App.Models;

public sealed partial class ProductionUnitsViewModel : ViewModelBase
{
    public ObservableCollection<MaintenanceEvent> MaintenanceSchedules => MaintenanceStore.MaintenanceSchedules;
}
