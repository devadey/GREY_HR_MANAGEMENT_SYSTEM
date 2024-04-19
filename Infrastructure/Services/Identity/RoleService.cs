using Common.Authorization;
using Common.Requests.Identity.Roles;
using Common.Responses.Identity.Roles;

namespace Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        this._roleManager = roleManager;
        this._userManager = userManager;
    }
    public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request)
    {
        var roleExists = await _roleManager.FindByNameAsync(request.RoleName);

        if (roleExists is not null)
        {
            return await ResponseWrapper.FailAsync("Role already exists.");
        }

        var role = new ApplicationRole
        {
            Name = request.RoleName,
            Description = request.RoleDescription
        };

        var identityResult = await _roleManager.CreateAsync(role);

        if (identityResult.Succeeded)
        {
            var mappedRole = new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
            };

            return await ResponseWrapper<RoleResponse>.SuccessAsync(mappedRole, "Role successfully created.");
        }
        return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescription(identityResult));
    }

    public async Task<IResponseWrapper> GetRoleByIdAsync(string id)
    {
        var roleExists = await _roleManager.FindByIdAsync(id);

        if (roleExists is null)
        {
            return await ResponseWrapper.FailAsync("Role does not exist.");
        }
        var mappedRole = new RoleResponse
        {
            Id = roleExists.Id,
            Name = roleExists.Name,
            Description = roleExists.Description,
        };

        return await ResponseWrapper<RoleResponse>.SuccessAsync(mappedRole);
    }

    public async Task<IResponseWrapper> GetRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        if (roles.Count > 0)
        {
            var allRoles = new List<RoleResponse>();
            foreach (var role in roles)
            {
                var mappedRole = new RoleResponse
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                 };

                allRoles.Add(mappedRole);
            }
            return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(allRoles);
        }
        return await ResponseWrapper.FailAsync("No roles exist.");
    }

    public async Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest request)
    {
        var roleExists = await _roleManager.FindByIdAsync(request.RoleId);

        if (roleExists is not null)
        {
            if (roleExists.Name != AppRoles.Admin)
            {
                roleExists.Name = request.RoleName;
                roleExists.Description = request.RoleDescription;

                var identityResult = await _roleManager.UpdateAsync(roleExists);

                if (identityResult.Succeeded)
                {
                    var updatedRole = new RoleResponse
                    {
                        Id = roleExists.Id,
                        Name = roleExists.Name,
                        Description = roleExists.Description,
                    };
                    return await ResponseWrapper<RoleResponse>.SuccessAsync(updatedRole, "Role has been successfully updated.");
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescription(identityResult));
            }
            return await ResponseWrapper.FailAsync("Cannot update admin role.");
        }
        return await ResponseWrapper.FailAsync("Role does not exist.");    
    }

    private List<string> GetIdentityResultErrorDescription(IdentityResult identityResult)
    {
        var errorDescription = new List<string>();

        foreach (var error in identityResult.Errors)
        {
            errorDescription.Add(error.Description);
        }

        return errorDescription;
    }

    public async Task<IResponseWrapper> TokenRequests(string roleId)
    {
        var roleInDb = await _roleManager.FindByIdAsync(roleId);

        if (roleInDb is not null)
        {
            var allPermissions = AppPermissions.AllPermission;
            var roleClaimResponse = new RoleClaimResponse
            {
                Role = new()
                {
                    Id = roleId,
                    Name = roleInDb.Name,
                    Description = roleInDb.Description,
                },
                RoleClaim = new()
            };

            var allPermissionsName = allPermissions.Select(x => x.Name == "Admin").ToList();
        }

        return await ResponseWrapper.FailAsync("Role does not exist.");
}
}
