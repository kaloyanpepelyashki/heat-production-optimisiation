using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Dv.App.Models;


/// Data transfer object, encapsulating the whole optimisation object. Used to send/receive optimisation from API.

/// 
public class OptimisationRunDto
{
    //Currently looks the same as the domain model, but can give flexibility later. Can allow for data transformations etc.
    
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