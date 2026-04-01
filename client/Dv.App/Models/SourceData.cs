using System;

namespace Dv.App.Models;

public class SourceData
{
    public int Id { get; set; }
    public int PeriodId { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public double HeatDemand { get; set; }
    public double ElectricityPrice { get; set; }
}