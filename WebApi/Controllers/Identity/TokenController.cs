using Application.Features.Identity.Queries;
using Application.Services.Identity;
using Common.Requests;
using MediatR;

namespace WebApi.Controllers.Identity;


[Route("api/[controller]")]
public class TokenController:BaseController<TokenController>
{
    private readonly ISender sender;
    private readonly ITokenService tokenService;

    public TokenController(ISender sender, ITokenService tokenService) : base(sender, tokenService)
    {
        this.sender = sender;
        this.tokenService = tokenService;
    }

    [HttpPost("get-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
    {
        var response = await sender.Send(new GetTokenQuery { TokenRequest = tokenRequest });
        //var response = await tokenService.GetTokenAsync(tokenRequest);
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }


    [HttpPost("get-refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await sender.Send(new GetRefreshTokenQuery { RefreshTokenRequest = refreshTokenRequest });
        //var response = await tokenService.GetRefreshTokenAsync(refreshTokenRequest);
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}

