using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Application.Features;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class ProductController(ILogger<DocumentController> logger, IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(TransferProductVm))]
    public async Task<IActionResult> SaveProduct([FromBody] TransferProductCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = (await mediator.Send<TransferProductVm>(command)).Result
    });

    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(AnalyzeProductImageVm))]
    public async Task<IActionResult> AnalyzeProductImage([FromBody] AnalyzeProductImageCommand command)
    => Ok(new ApiResponse<AnalyzeProductImageVm>()
    {
        Successful = true,
        Value = await mediator.Send(command)
    });

    [HttpPost("[action]")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxSearchRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxSearchRequestBytes)]
    [ProducesDefaultResponseType(typeof(SearchProductsByImageVm))]
    public async Task<IActionResult> SearchProductsByImage([FromForm] SearchProductsByImageQuery query)
    => Ok(new ApiResponse<SearchProductsByImageVm>()
    {
        Successful = true,
        Value = await mediator.Send(query)
    });

    // up to 8 images of 10 MB each plus form fields
    private const long MaxSearchRequestBytes = 90L * 1024 * 1024;
}
