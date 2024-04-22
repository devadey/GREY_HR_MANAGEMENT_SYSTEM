﻿
namespace Application.Features.Identity.Roles.Queries;

public class GetPermissionsQuery : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetPermissionsQueryHandler(IRoleService roleService)
    {
        this._roleService = roleService;
    }
    public async Task<IResponseWrapper> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        return await _roleService.GetPermissionsAsync(request.RoleId);
    }
}
