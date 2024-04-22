using Common.Requests.Identity.Roles;

namespace Application.Features.Identity.Roles.Commands;

public class UpdateRolesPermissionCommand : IRequest<IResponseWrapper>
{
    public UpdateRolePermissionRequest UpdateRolePermissionRequest { get; set; }
}

public class UpdateRolesPermissionCommandHandler : IRequestHandler<UpdateRolesPermissionCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public UpdateRolesPermissionCommandHandler(IRoleService roleService)
    {
        this._roleService = roleService;
    }
    public async Task<IResponseWrapper> Handle(UpdateRolesPermissionCommand request, CancellationToken cancellationToken)
    {
        return await _roleService.UpdateRolePermissionsAsync(request.UpdateRolePermissionRequest);
    }
}
