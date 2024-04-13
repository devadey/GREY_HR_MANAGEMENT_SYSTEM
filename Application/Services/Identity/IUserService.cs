namespace Application.Services.Identity;

public interface IUserService
{
    Task<IResponseWrapper> ResgisterUserAsync(UserRegistrationRequest request);
    Task<IResponseWrapper> GetUserByIdAsync(string userId);
    Task<IResponseWrapper> GetAllUsersAsync();
}
