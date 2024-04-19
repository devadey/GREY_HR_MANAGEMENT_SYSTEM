﻿
namespace Application.Features.Identity.Roles.Queries;

public class GetRoleByIdQuery : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetRoleByIdQueryHandler(IRoleService roleService)
    {
        this._roleService = roleService;
    }
    public async Task<IResponseWrapper> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        return await _roleService.GetRoleByIdAsync(request.RoleId);
    }
}
