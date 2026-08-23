using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;
using Silo.Identity.Server;
using Silo.Identity.Server.Services;

namespace Silo.Api.Controllers.v2;
public class AccountController(
    ILogger<AccountController> logger,
    IdentityBusiness business,
    IJwtService jwtService,
    IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [AllowAnonymous]
    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthenticateByPassword(ApiAuthenticateDto request)
        => Ok(new ApiResponse()
        {
            Successful = true,
            Value = await jwtService.AuthenticateAsync(request)
        });

    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(ApiResponse<GenerateUserTokenVm>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerateUserToken(
        [FromBody] GenerateUserTokenCommand command,
        CancellationToken cancellationToken)
        => Ok(new ApiResponse<GenerateUserTokenVm>
        {
            Successful = true,
            Value = await mediator.Send<GenerateUserTokenVm>(command, cancellationToken)
        });

    [HttpGet("[action]")]
    [ProducesDefaultResponseType(typeof(ApiResponse<GetUserTokensVm>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserTokens(
        [FromBody] GetUserTokensQuery query,
        CancellationToken cancellationToken)
        => Ok(new ApiResponse<GetUserTokensVm>
        {
            Successful = true,
            Value = await mediator.Send<GetUserTokensVm>(query, cancellationToken)
        });

    [HttpDelete("[action]")]
    [ProducesDefaultResponseType(typeof(ApiResponse<DeleteUserTokenVm>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteUserToken(
        [FromBody] DeleteUserTokenCommand command,
        CancellationToken cancellationToken)
        => Ok(new ApiResponse<DeleteUserTokenVm>
        {
            Successful = true,
            Value = await mediator.Send<DeleteUserTokenVm>(command, cancellationToken)
        });

    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(ApiResponse<AddBulkUserClaimsVm>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddBulkUserClaims(
        [FromBody] AddBulkUserClaimsCommand command,
        CancellationToken cancellationToken)
        => Ok(new ApiResponse<AddBulkUserClaimsVm>
        {
            Successful = true,
            Value = await mediator.Send<AddBulkUserClaimsVm>(command, cancellationToken)
        });
}
