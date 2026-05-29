using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Dv.App.Models;

public class OptimisationRunDto
{
    //The Id will stay, as the same DTO is used for transfer to DV
    [JsonPropertyName("id")]
    public int? Id { get; set; }
    [JsonPropertyName("timeFrom")]
    public DateTime TimeFrom { get; set; }
    [JsonPropertyName("timeTo")]
    public DateTime TimeTo { get; set; }
    [JsonPropertyName("scenario")]
    public string Scenario { get; set; }
    [JsonPropertyName("periodType")]
    public string PeriodType { get; set; }
    [JsonPropertyName("optimisationResultsHourly")]
    public List<OptimisationResultsHourlyDto> optimisationResultsHourly{get; set;}
    
}