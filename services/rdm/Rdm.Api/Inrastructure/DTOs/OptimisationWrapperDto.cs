namespace Rdm.Api.Inrastructure.DTOs;

using System.Text.Json.Serialization;

public class OptimisationWrapperDto
{
    [JsonPropertyName("optRun")]
    public OptimisationRunDto OptimisationRun { get; set; }

    [JsonPropertyName("optResultsHourly")]
    public List<OptimisationResultHourlyDto> OptimisationResultsHourly { get; set; }
}