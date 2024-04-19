using Common.Requests.Identity.Roles;

namespace Application.Features.Identity.Roles.Commands;

public class UpdateRoleCommand : IRequest<IResponseWrapper>
{
    public UpdateRoleRequest UpdateRoleRequest { get; set;}
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public UpdateRoleCommandHandler(IRoleService roleService)
    {
        this._roleService = roleService;
    }
    public async Task<IResponseWrapper> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        return await _roleService.UpdateRoleAsync(request.UpdateRoleRequest);
    }
}
