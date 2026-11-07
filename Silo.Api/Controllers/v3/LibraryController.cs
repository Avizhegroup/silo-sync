using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v3;


[AllowAnonymous]

public class LibraryController(ILogger<LibraryController> logger
    , IConfiguration configuration
    , IMediator mediator) : SiloBaseControllerVersion3(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> SaveBooks(SaveBooksCommand request)
    {
        //var response = await mediator.Send<SaveBooksVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = true
        });
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> BooksOps(SaveBookOpsCommand request)
    {
        //var response = await mediator.Send<SaveBookOpsVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = true
        });
    }

}
