namespace Infrastructure.Services.Helpers;

public class TokenServiceHelpers
{
    private readonly AppConfiguration _appConfiguration;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenServiceHelpers( RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IOptions<AppConfiguration> appConfiguration)
    {
        _appConfiguration = appConfiguration.Value;
        this._roleManager = roleManager;
        this._userManager = userManager;

    }
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg
            .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid Token");
        }

        return principal;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    public async Task<string> GenerateJWTAsync(ApplicationUser user)
    {
        // Generate Token
        // Encrypt the token generated.
        var token = GenerateEncrytedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        return token;
    }

    public string GenerateEncrytedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var Token = new JwtSecurityToken(
        claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes),
            signingCredentials: signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        var encryptedToken = handler.WriteToken(Token);
        return encryptedToken;

    }

    public SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfiguration.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();

        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            var currentRole = await _roleManager.FindByNameAsync(role);
            var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
            permissionClaims.AddRange(allPermissionsForCurrentRole);
        }

        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Name, user.FirstName),
        new(ClaimTypes.Surname, user.LastName),
        new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
    }
        .Union(userClaims)
        .Union(roleClaims)
        .Union(permissionClaims);


        return claims;
    }
}
