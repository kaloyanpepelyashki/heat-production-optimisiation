namespace Rdm.Api.Inrastructure.API;

public class ApiResponseModel<T>
{
    public string? Message { get; set; }

    public string? Error { get; set; }

    public int? Count { get; set; }

    public T Data { get; set; }


    public ApiResponseModel(string message, T data, int? count = null, string error = null)
    {
        this.Message = message;
        this.Data = data;
        this.Count = count;
        this.Error = error;
    }
}