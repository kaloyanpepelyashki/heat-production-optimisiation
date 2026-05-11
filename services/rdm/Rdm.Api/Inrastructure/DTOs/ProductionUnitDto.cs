using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Rdm.Api.Inrastructure.DTOs;

public class ProductionUnitDto
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }
    [JsonPropertyName("unitId")]
    public int ProductionUnitId { get; set; }
    [JsonPropertyName("unitType")]
    public string ProductionUnitType { get; set; }
    [JsonPropertyName("capacityOutput")]
    public double Capacity { get; set; }
}