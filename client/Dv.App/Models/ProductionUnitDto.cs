using System.Text.Json.Serialization;

namespace Dv.App.Models;

public class ProductionUnitDto
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }
    [JsonPropertyName("productionUnitId")]
    public int ProductionUnitId { get; set; }
    [JsonPropertyName("productionUnitType")]
    public string ProductionUnitType { get; set; }
    [JsonPropertyName("capacity")]
    public double Capacity { get; set; }

    [JsonPropertyName("heatProduction")]
    public double HeatProduction { get; set; }
}