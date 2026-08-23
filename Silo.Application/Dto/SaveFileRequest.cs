using Microsoft.AspNetCore.Http;

namespace Silo.Application.Dto;
public class SaveFileRequest
{
    public IFormFile File { get; set; }
}
