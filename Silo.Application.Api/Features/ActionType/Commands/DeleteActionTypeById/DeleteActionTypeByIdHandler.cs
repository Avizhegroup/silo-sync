namespace Silo.Application.Api.Features;
public class DeleteActionTypeByIdHandler(WmsApiContext context) : IRequestHandler<DeleteActionTypeByIdCommand, DeleteActionTypeByIdVm>
{
    public async Task<DeleteActionTypeByIdVm> Handle(DeleteActionTypeByIdCommand request, CancellationToken cancellationToken)
    => new DeleteActionTypeByIdVm
    {
        Result = (await context.ActionTypes
                              .Where(p => p.Id == request.Id)
                              .ExecuteDeleteAsync(cancellationToken)) > 0
    };
}
