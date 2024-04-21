using Common.Requests.Identity.Roles;
using Common.Requests.Identity.Users;

namespace Application.Services.Identity;

public interface IUserService
{
    Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request);
    Task<IResponseWrapper> GetUserByIdAsync(string userId);
    Task<IResponseWrapper> GetAllUsersAsync();
    Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request);
    Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request);
    Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request);
    Task<IResponseWrapper> GetUserRolesAsync(string userId);
    Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRoleRequest request);
}
