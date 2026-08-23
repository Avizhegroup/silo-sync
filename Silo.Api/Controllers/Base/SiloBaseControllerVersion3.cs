using Microsoft.AspNetCore.Authorization;
using Silo.Api.Business;

namespace Silo.Base.Controllers.Base;

[Authorize(AuthenticationSchemes = "DatabaseToken")]
public class SiloBaseControllerVersion3 : SiloBaseController
{
    public SiloBaseControllerVersion3(ILogger<SiloBaseControllerVersion3> logger) : base(logger)
    {

    }

    public SiloBaseControllerVersion3(ILogger<SiloBaseControllerVersion3> logger, ProjectBusiness business) : base(logger, business)
    {

    }
}
