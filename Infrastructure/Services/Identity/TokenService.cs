namespace Infrastructure.Services.Identity;

public class TokenService : ItokenService
{
    
    public Task<TokenResponse> GetTokenAsync(TokenRequest tokenRequest)
    {
        throw new NotImplementedException();
    }

    public Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        throw new NotImplementedException();
    }
}
