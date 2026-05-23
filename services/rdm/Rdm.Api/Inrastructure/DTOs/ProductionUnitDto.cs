namespace Rdm.Api.Inrastructure.DTOs;

using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class ProductionUnitDto
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("unitId")]
    public int ProductionUnitId { get; set; }

    [JsonPropertyName("unitType")]
    public string ProductionUnitType { get; set; }

    [JsonPropertyName("heatProduction")]
    public double HeatProduction { get; set; }

    [JsonPropertyName("electricityConsumption")]
    public double ElectricityConsumption { get; set; }

    [JsonPropertyName("expenses")]
    public double Expenses { get; set; }

    [JsonPropertyName("co2Emissions")]
    public double Co2Emissions { get; set; }

    [JsonPropertyName("capacityOutput")]
    public double Capacity { get; set; }
}