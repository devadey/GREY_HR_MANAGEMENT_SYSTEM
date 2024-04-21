
namespace Application.Features.Identity.Users.Commands;

public class UpdateUserRolesCommand : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
