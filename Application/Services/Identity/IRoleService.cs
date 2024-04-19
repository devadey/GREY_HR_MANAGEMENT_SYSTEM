using Common.Requests.Identity.Roles;

namespace Application.Services.Identity;

public interface IRoleService
{
    Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request);
    Task<IResponseWrapper> GetRolesAsync();
    Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest request);
    Task<IResponseWrapper> GetRoleByIdAsync(string id);
}
