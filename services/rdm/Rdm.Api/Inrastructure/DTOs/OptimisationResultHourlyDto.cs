namespace Rdm.Api.Inrastructure.DTOs;

using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class OptimisationResultHourlyDto
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

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

    [JsonPropertyName("units")]
    public List<ProductionUnitDto> ProductionUnits { get; set; }
}