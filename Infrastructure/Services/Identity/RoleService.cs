using Common.Authorization;
using Common.Requests.Identity.Roles;
using Common.Responses.Identity.Roles;

namespace Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ApplicationDbContext _dbContext;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService, ApplicationDbContext dbContext)
    {
        this._roleManager = roleManager;
        this._userManager = userManager;
        this._currentUserService = currentUserService;
        this._dbContext = dbContext;
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

    public async Task<IResponseWrapper> DeleteRoleAsync(string roleId)
    {
        var roleInDb = await _roleManager.FindByIdAsync(roleId);

        if (roleInDb is not null)
        {
            var allUsers = await _userManager.Users.ToListAsync();

            foreach (var user in allUsers)
            {
                var currentlyLoggedUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
                if (!await _userManager.IsInRoleAsync(currentlyLoggedUser, AppRoles.Admin))
                {
                    return await ResponseWrapper<string>.FailAsync($"Only an admin user is permitted to delete a role.");
                }

                if (roleInDb.Name.Equals(AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
                {
                    return await ResponseWrapper<string>.FailAsync("You're not authorized to delete an Admin role");
                }

                if (await _userManager.IsInRoleAsync(user, roleInDb.Name))
                {
                    return await ResponseWrapper<string>.FailAsync($"{roleInDb.Name} role is currently assigned to a user and cannot be deleted.");
                }

                var identityResult = await _roleManager.DeleteAsync(roleInDb);
                if (identityResult.Succeeded)
                {
                    return await ResponseWrapper<string>.SuccessAsync("Role has been successfully deleted.");
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescription(identityResult));
            }
        }      
        return await ResponseWrapper<string>.FailAsync("Role does not exist.");
    }

    public async Task<IResponseWrapper> GetPermissionsAsync(string roleId)
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
                RoleClaim = new() { }
            };

            var currentRoleClaims = await GetAllClaimsForRoleAsync(roleId);
            var allPermissionsNames = allPermissions.Select(x => x.Name).ToList();  
            var currentRoleClaimsValues = currentRoleClaims.Select(crc => crc.ClaimValue).ToList();

            var currentlyAssignedRoleClaimsNames = allPermissionsNames.Intersect(currentRoleClaimsValues).ToList();

            foreach (var permission in allPermissions)
            {
                if (currentlyAssignedRoleClaimsNames.Any(carc => carc == permission.Name))
                {
                    roleClaimResponse.RoleClaim.Add(new RoleClaimViewModel
                    {
                        RoleId = roleId,
                        ClaimType = AppClaim.Permission,
                        ClaimValue = permission.Name,
                        Description = permission.Description,
                        Group = permission.Group,
                        IsAssignedToRole = true
                    });
                } 
                else
                {
                    roleClaimResponse.RoleClaim.Add(new RoleClaimViewModel
                    {
                        RoleId = roleId,
                        ClaimType = AppClaim.Permission,
                        ClaimValue = permission.Name,
                        Description = permission.Description,
                        Group = permission.Group,
                        IsAssignedToRole = false
                    });
                }
            }
            return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }
        return await ResponseWrapper<string>.FailAsync("Role does not exist.");
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

    public async Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRolePermissionRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.RoleId);
        if (roleInDb is not null)
        {
            if (roleInDb.Name == AppRoles.Admin)
            {
                return await ResponseWrapper<string>.FailAsync("Cannot change persmission for this role");
            }

            var permissionsToBeAssigned = request.RolesClaims.Where(c => c.IsAssignedToRole is true).ToList();

            var currentlyAssignedClaims = await _roleManager.GetClaimsAsync(roleInDb);

            foreach (var claim in currentlyAssignedClaims)
            {
                await _roleManager.RemoveClaimAsync(roleInDb, claim);
            }

            foreach (var permission in permissionsToBeAssigned)
            {
                var mappedClaims = new ApplicationRoleClaim
                {
                    RoleId = roleInDb.Id,
                    ClaimType = permission.ClaimType,
                    ClaimValue = permission.ClaimValue,
                    Description = permission.Description,
                    Group = permission.Group,
                };

                await _dbContext.RoleClaims.AddAsync(mappedClaims);
            }
            await _dbContext.SaveChangesAsync();
            return await ResponseWrapper.SuccessAsync("Role permissions updated successfully.");
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

    private async Task<List<RoleClaimViewModel>> GetAllClaimsForRoleAsync(string roleId)
    {
        var roleClaims = await _dbContext.RoleClaims.Where(rc => rc.RoleId == roleId).ToListAsync();
        var mappedRoleClaims = new List<RoleClaimViewModel>();
        if (roleClaims.Count > 0)
        {
            foreach (var roleClaim in roleClaims)
            {
                var newRoleClaimVM = new RoleClaimViewModel
                {
                    RoleId = roleClaim.RoleId,
                    ClaimType = roleClaim.ClaimType,
                    ClaimValue = roleClaim.ClaimValue,
                    Description = roleClaim.Description,
                    Group = roleClaim.Group,
                };
                mappedRoleClaims.Add(newRoleClaimVM);
            }
            return mappedRoleClaims;    
        }
        return new List<RoleClaimViewModel>();
    }

   
}
