using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class DocumentController (ILogger<DocumentController> logger,IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(TransferDocumentVm))]
    public async Task<IActionResult> SaveDocument([FromBody] TransferDocumentCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = (await mediator.Send<TransferDocumentVm>(command)).Result
    });
}
