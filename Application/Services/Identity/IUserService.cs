namespace Application.Services.Identity;

public interface IUserService
{
    Task<IResponseWrapper> ResgisterUserAsync(UserRegistrationRequest request);
}
