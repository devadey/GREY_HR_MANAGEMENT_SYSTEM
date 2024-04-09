namespace Application.Services.Identity;

public interface ItokenService
{
    Task<TokenResponse> GetTokenAsync(TokenRequest tokenRequest);
    Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}
