namespace Infrastructure.Context;

public class ApplicationDbSeeder
{ 
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;

    public ApplicationDbSeeder(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task SeedDataAsync() 
    {
        await CheckAndApplyPendingMigrationsAsync();
        await SeedRolesAsync();
        SeedAdminToDatabaseAsync().GetAwaiter().GetType();
    //Seed Roles

    }
    private async Task CheckAndApplyPendingMigrationsAsync()
    {
        //Check if the Database is connected and and if there is any pending migration.
        if (await _dbContext.Database.CanConnectAsync() && _dbContext.Database.GetPendingMigrations().Any())
        {
            await _dbContext.Database.MigrateAsync();
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in AppRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                role = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} Role."
                };

                await _roleManager.CreateAsync(role);
               
            }

            if (roleName == AppRoles.Admin)
            {
                await AssignPermissionToRoleAsync(role, AppPermissions.AdminPermissions);
            }
            else if (roleName == AppRoles.Basic)
            {
                await AssignPermissionToRoleAsync(role, AppPermissions.BasicPermissions);
            }
        }
    }

    private async Task AssignPermissionToRoleAsync(ApplicationRole role, IReadOnlyList<AppPermission> permissions)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var permission in permissions)
        {
            if (!currentClaims.Any(claim => claim.Type == AppClaim.Permission && claim.Value == permission.Name))
            {
                await _dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group
                });
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedAdminToDatabaseAsync() 
    {
        var adminUsername = AppCredentials.Email[..AppCredentials.Email.IndexOf("a")].ToLowerInvariant();
        var adminUser = new ApplicationUser
        {
            FirstName = "Adey",
            LastName = "Sam",
            UserName = adminUsername,
            Email = AppCredentials.Email,
            NormalizedEmail = AppCredentials.Email.ToUpperInvariant(),
            NormalizedUserName = adminUsername.ToUpper(),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,

        };

        if (!await _userManager.Users.AnyAsync(u => u.Email == AppCredentials.Email))
        {
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, AppCredentials.Password);
            await _userManager.CreateAsync(adminUser);
        }

        if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Basic))
        {
            await _userManager.AddToRolesAsync(adminUser, AppRoles.DefaultRoles);
        }
            
        }

    }
