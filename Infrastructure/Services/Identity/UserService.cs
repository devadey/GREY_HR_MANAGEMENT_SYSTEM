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
