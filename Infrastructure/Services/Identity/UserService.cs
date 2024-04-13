using Azure.Core;
using Common.Requests.Identity;
using Common.Responses.Identity;

namespace Infrastructure.Services.Identity;

public class UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<IResponseWrapper> ResgisterUserAsync(UserRegistrationRequest request)
    {
        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);

        if (userWithSameEmail is not null)
        {
            return await ResponseWrapper.FailAsync("User email has been taken.");
        }

        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);

        if (userWithSameUserName is not null)
        {
            return await ResponseWrapper.FailAsync("Username already taken.");
        }

        
        var newUser = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.ActivateUser,
            UserName = request.UserName,
            EmailConfirmed = request.AutoCofirmEmail

        };

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        newUser.PasswordHash = passwordHasher.HashPassword(newUser, request.Password);

        var userResult = await _userManager.CreateAsync(newUser);


        var mappedUser = new UserResponse
        {
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email,
            PhoneNumber = newUser.PhoneNumber,
            IsActive = newUser.IsActive,
            UserName = newUser.UserName,
            EmailConfirmed = newUser.EmailConfirmed

        };

        if (userResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);

            return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser, "User registered successfully.");
        };

        return await ResponseWrapper<string>.FailAsync("Username registration failed.");
    }
}
