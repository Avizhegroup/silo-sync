using Microsoft.AspNetCore.Authorization;
using Silo.Api.Business;

namespace Silo.Base.Controllers.Base;

[Authorize(AuthenticationSchemes = "Bearer")]
public class SiloBaseControllerVersion2 : SiloBaseController
{
    public SiloBaseControllerVersion2(ILogger<SiloBaseControllerVersion2> logger) : base(logger)
    {

    }

    public SiloBaseControllerVersion2(ILogger<SiloBaseControllerVersion2> logger, ProjectBusiness business) : base(logger, business)
    {

    }
}
