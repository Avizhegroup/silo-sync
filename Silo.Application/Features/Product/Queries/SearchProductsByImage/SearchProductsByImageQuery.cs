using Microsoft.AspNetCore.Http;

namespace Silo.Application.Features;

/// <summary>Sent as multipart/form-data: the images travel as raw files, never as base64 text.</summary>
public class SearchProductsByImageQuery : IRequest<SearchProductsByImageVm>
{
    public List<IFormFile> Images { get; set; } = [];
    public int MaxResults { get; set; } = 20;
    public double MinScore { get; set; } = 5;
}
