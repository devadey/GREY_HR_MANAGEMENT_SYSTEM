using Common.Requests.Identity.Users;

namespace Application.Services.Identity;

public interface IUserService
{
    Task<IResponseWrapper> ResgisterUserAsync(UserRegistrationRequest request);
    Task<IResponseWrapper> GetUserByIdAsync(string userId);
    Task<IResponseWrapper> GetAllUsersAsync();
    Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request);
    Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request);
    Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request);
}
