namespace Rdm.Api.Inrastructure.API;

public class ApiResponseModel<T>
{
    public string? Message { get; set; }
    public string? Error { get; set; }
    public int? Count { get; set; }
    
    public T Data { get; set; }


    public ApiResponseModel(string message, T data, int? count = null, string error = null)
    {
        Message = message;
        Data = data;
        Count = count;
        Error = error;
    }
}