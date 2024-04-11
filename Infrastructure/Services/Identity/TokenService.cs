namespace Infrastructure.Services.Identity;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenServiceHelpers _helper;

    public TokenService(UserManager<ApplicationUser> userManager, TokenServiceHelpers helper)
    {
        this._userManager = userManager;
        this._helper = helper;
    }

    public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest)
    {
        // Validate user
        // Check user
        // Check if the use ris active
        // Check if email is confirmed
        // Check password
        var user = await _userManager.FindByEmailAsync(tokenRequest.Email);
        if (user is null)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid user credentials.");
        }

        if (!user.IsActive)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("User is not active. Kindly reach out to the administrators for directions.");
        }

        if (!user.EmailConfirmed)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("User email is not confirmed. Kindly confirm your email before you proceed.");
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);
        if (!isValidPassword)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid user credentials.");
        }

        // Generate Refresh token
        user.RefreshToken = _helper.GenerateRefreshToken();
        user.RefreshTokenExpiryDate = DateTime.Now.AddDays(6);

        // Update the user
        await _userManager.UpdateAsync(user);

        // Generate new Token
        var token = await _helper.GenerateJWTAsync(user);
        // Return token
        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = _helper.GenerateRefreshToken(),
            RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
        };

        return await ResponseWrapper<TokenResponse>.SuccessAsync(response);

    }

    public async Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        if (refreshTokenRequest is null)
            await ResponseWrapper<TokenResponse>.FailAsync("Invalid Client Token.");

        var userPrincipal = _helper.GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user is null)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("User not found.");
        }
        if (user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryDate <= DateTime.Now)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Client Token.");
        }

        var token = _helper.GenerateEncrytedToken(_helper.GetSigningCredentials(), await _helper.GetClaimsAsync(user));
        user.RefreshToken = _helper.GenerateRefreshToken();
        await _userManager.UpdateAsync(user);


        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
        };

        return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
    }
}

