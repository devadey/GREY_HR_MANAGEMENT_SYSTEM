using Common.Requests.Identity.Users;
using Common.Responses.Identity.Users;

namespace Infrastructure.Services.Identity;

public class UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<IResponseWrapper> GetUserByIdAsync(string userId)
    {
        var userInDb = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == userId);

        
        if (userInDb is not null)
        {
            var mappedUser = new UserResponse
            {
                UserName = userInDb.UserName,
                FirstName = userInDb.FirstName,
                LastName = userInDb.LastName,
                PhoneNumber = userInDb.PhoneNumber,
                Email = userInDb.Email,
                IsActive = userInDb.IsActive,
                EmailConfirmed = userInDb.EmailConfirmed,
            };

            return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
        }

        return await ResponseWrapper<string>.FailAsync("User does not exist.");
    }


    public async Task<IResponseWrapper> GetAllUsersAsync()
    {
        var userResult = await _userManager.Users.ToListAsync();


        if (userResult is not null)
        {
            var mappedUser = new List<UserResponse>();
            foreach (var user in userResult)
            {
                
                var newUser = new UserResponse
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                };
                mappedUser.Add(newUser);
            }
            return await ResponseWrapper<List<UserResponse>>.SuccessAsync(mappedUser);
        }

        return await ResponseWrapper<string>.FailAsync("No result found.");
    }

    public async Task<IResponseWrapper> ResgisterUserAsync(UserRegistrationRequest request)
    {
        if (request.ConfirmPassword != request.Password)
        {
            return await ResponseWrapper.FailAsync("Password mismatched. Kindly review your password.");
        }

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

    public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request)
    {
        var userInDb = await _userManager.FindByIdAsync(request.UserId);

        if (userInDb == null)
        {
            return await ResponseWrapper.FailAsync("User does not exist");
        }

        userInDb.FirstName = request.FirstName;
        userInDb.LastName = request.LastName;
        userInDb.PhoneNumber = request.PhoneNumber;

        var updatedResult = await _userManager.UpdateAsync(userInDb);

        if (updatedResult.Succeeded)
        {
            var updatedUser = new UserResponse
            {
                UserName = userInDb.UserName,
                Email = userInDb.Email,
                PhoneNumber = userInDb.PhoneNumber,
                IsActive = userInDb.IsActive,
                EmailConfirmed = userInDb.EmailConfirmed,
                FirstName = userInDb.FirstName,
                LastName = userInDb.LastName,
            };

            return await ResponseWrapper<UserResponse>.SuccessAsync(updatedUser, "User details has been successfully updated.");
        }

        return await ResponseWrapper<UserResponse>.FailAsync("Error occurred the user details.");


    }

    public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request)
    {

        var userinDb = await _userManager.FindByIdAsync(request.UserId);

        if (userinDb is null)
        {
            return await ResponseWrapper<UserResponse>.FailAsync("user does not exist.");
        } 
        if(userinDb.IsActive == false)
        {
            return await ResponseWrapper<UserResponse>.FailAsync("Opps.....This user is not active.");
        }
        var checkPassword = await _userManager.CheckPasswordAsync(userinDb, request.CurrentPassword);

        if (checkPassword != true)
        {
            return await ResponseWrapper<UserResponse>.FailAsync("Current password is incorrect.");
        }

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return await ResponseWrapper<UserResponse>.FailAsync("Your new password and the confirm password are not match.");
        }

        if (request.CurrentPassword == request.NewPassword)
        {
            return await ResponseWrapper<UserResponse>.FailAsync("Your new password cannot be the same as your current password.");
        }

        var updateResult = await _userManager.ChangePasswordAsync(userinDb, request.CurrentPassword, request.NewPassword);

        if (updateResult.Succeeded)
        {
            var mappedUser = new UserResponse
            {
                UserName = userinDb.UserName
            };

            return await ResponseWrapper.SuccessAsync($"{mappedUser.UserName} password has successfully been changed.");
        }
        return await ResponseWrapper.FailAsync("Error occurred and couldn't complete the task.");
    }

    public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request)
    {
        var userInDb = await _userManager.FindByIdAsync(request.UserId);

        if (userInDb == null)
        {
            return await ResponseWrapper.FailAsync("User does not exist");
        }

        if (userInDb.IsActive == request.UserStatus)
        {
            return await ResponseWrapper.FailAsync("User status remain unchanged. Please check your input and change the status.");
        }

        userInDb.IsActive = request.UserStatus;

        var userResult = await _userManager.UpdateAsync(userInDb);

        if (userResult.Succeeded)
        {
            string userStatus;
            if (userInDb.IsActive == true)
            {
                userStatus = "ACTIVATED";
            } else
            {
                userStatus = "DEACTIVATED";
            }

            var newUser = new UserResponse
            {
                UserName = userInDb.UserName,
                FirstName = userInDb.FirstName,
                LastName = userInDb.LastName,
                PhoneNumber = userInDb.PhoneNumber,
                Email = userInDb.Email,
                EmailConfirmed = userInDb.EmailConfirmed,
                IsActive = userInDb.IsActive,
            };

            return await ResponseWrapper<UserResponse>.SuccessAsync(newUser, $"{userInDb.UserName} has been successfully {userStatus}.");
        }
        return await ResponseWrapper.FailAsync("User status could not be changed");
    }
}
