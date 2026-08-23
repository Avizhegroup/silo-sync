namespace Silo.Api.External.Sharif.Models;

public class SharifApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public SharifErrorResponse? Error { get; set; }
    public int StatusCode { get; set; }
}
