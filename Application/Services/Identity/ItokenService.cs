using Common.Requests.Identity.Token;

namespace Application.Services.Identity;

public interface ITokenService
{
    Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest);
    Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}
