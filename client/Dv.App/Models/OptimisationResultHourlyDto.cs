using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Dv.App.Models;



/// Data transfer object - nested in the optimisation run object. 

public class OptimisationResultsHourlyDto
{
    //Currently looks the same as the domain model, but can give flexibility later. Can allow for data transofrmations etc.
    [JsonPropertyName("id")]
    public int? Id { get; set; }
    [JsonPropertyName("optimisation_run_id")]
    public int? OptimizationRunId { get; set; }
    [JsonPropertyName("heatProduction")]
    public double HeatProduction { get; set; }
    [JsonPropertyName("electricityConsumption")]
    public double ElectricityConsumption { get; set; }
    [JsonPropertyName("co2Emissions")]
    public double Co2Emissions { get; set; }
    [JsonPropertyName("expenses")]
    public double Expenses { get; set; }
    [JsonPropertyName("timeFrom")]
    public DateTime TimeFrom { get; set; }
    [JsonPropertyName("timeTo")]
    public DateTime TimeTo { get; set; }

    [JsonPropertyName("productionUnits")]
    public List<ProductionUnitDto> ProductionUnits { get; set; }
}