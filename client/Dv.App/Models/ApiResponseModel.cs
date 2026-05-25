using System.Text.Json.Serialization;

namespace Dv.App.Models;

public class ApiResponseModel<T>
{
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("error")]
    public string Error { get; set; }
    
    [JsonPropertyName("count")]
    public int? Count { get; set; }
    
    [JsonPropertyName("data")]
    public T Data { get; set; }
}
