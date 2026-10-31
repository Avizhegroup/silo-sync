using MediatR;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v3;
public class TeamYarController (ILogger<SharifController> logger
    , IConfiguration configuration, IMediator mediator) : SiloBaseControllerVersion3(logger)
{

}
