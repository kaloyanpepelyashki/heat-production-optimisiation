namespace Dv.App.Models;

using System;
using System.Text.Json.Serialization;

public sealed class OptimizationRequestDto
{
    [JsonPropertyName("scenarioId")]
    public int ScenarioId { get; set; }

    [JsonPropertyName("periodId")]
    public int PeriodId { get; set; }

    [JsonPropertyName("maintenanceId")]
    public int MaintenanceId { get; set; }

    [JsonPropertyName("timeFrom")]
    public DateTime TimeFrom { get; set; }

    [JsonPropertyName("timeTo")]
    public DateTime TimeTo { get; set; }
}