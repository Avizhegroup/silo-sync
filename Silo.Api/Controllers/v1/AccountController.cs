using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;
using Silo.Identity.Server.Services;
using Silo.Identity.Server;

namespace Silo.Api.Controllers.v1;
public class AccountController : SiloBaseController
{
    private readonly ILogger<AccountController> logger;
    private readonly IdentityBusiness business;
    private readonly IJwtService jwtService;

    public AccountController(ILogger<AccountController> logger
        , IdentityBusiness business
        , IJwtService jwtService) : base(logger)
    {
        this.logger = logger;
        this.business = business;
        this.jwtService = jwtService;
    }

    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(ApiResponse<>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthenticateByPassword(ApiAuthenticateDto authInfo)
    => Ok(new ApiResponse<string>()
    {
        Successful = true,
        Value = await jwtService.AuthenticateAsync(authInfo)
    });
}
