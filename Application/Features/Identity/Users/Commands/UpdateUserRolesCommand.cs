
using Common.Requests.Identity.Roles;

namespace Application.Features.Identity.Users.Commands;

public class UpdateUserRolesCommand : IRequest<IResponseWrapper>
{
    public UpdateUserRoleRequest UpdateUserRoleRequest { get; set; }
}

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public UpdateUserRolesCommandHandler(IUserService userService)
    {
        this._userService = userService;
    }
    public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        return await _userService.UpdateUserRolesAsync(request.UpdateUserRoleRequest);
    }
}
