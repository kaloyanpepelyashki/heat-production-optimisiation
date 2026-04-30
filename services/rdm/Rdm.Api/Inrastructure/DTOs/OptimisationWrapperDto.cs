using System.Text.Json.Serialization;

namespace Rdm.Api.Inrastructure.DTOs;

public class OptimisationWrapperDto
{
    [JsonPropertyName("optRun")]
    public OptimisationRunDto OptimisationRun { get; set; }
    [JsonPropertyName("optResultsHourly")]
    public List<OptimisationResultHourlyDto> OptimisationResultsHourly { get; set; }
    
}