
using Common.Requests.Identity.Roles;

namespace Application.Features.Identity.Roles.Commands;

public class CreateRoleCommand : IRequest<IResponseWrapper>
{
    public CreateRoleRequest CreateRoleRequest { get; set; }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public CreateRoleCommandHandler(IRoleService roleService)
    {
        this._roleService = roleService;
    }
    public async Task<IResponseWrapper> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        return await _roleService.CreateRoleAsync(request.CreateRoleRequest);
    }
}
