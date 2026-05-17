using System;
using System.Collections.ObjectModel;

namespace Dv.App.Models;

// Storing data to view in ProductionUnitsView UI
public class MaintenanceEvent
{
    public int MaintenanceId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public int BoilerId { get; set; }
    public string BoilerType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Period { get; set; } = string.Empty;
    public string Scenario { get; set; } = string.Empty;
}

public static class MaintenanceStore
{
    public static ObservableCollection<MaintenanceEvent> MaintenanceSchedules { get; } = new();
}