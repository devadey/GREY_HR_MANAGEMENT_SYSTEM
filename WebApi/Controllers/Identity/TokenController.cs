using Application.Features.Identity.Queries;

namespace WebApi.Controllers.Identity;


[Route("api/[controller]")]
public class TokenController:BaseController<TokenController>
{

    [HttpPost("get-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetToken([FromBody] TokenRequest tokenRequest)
    {
        var response = await MediatorSender.Send(new GetTokenQuery { TokenRequest = tokenRequest});
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }


    [HttpPost("get-refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await MediatorSender.Send(new GetRefreshTokenQuery { RefreshTokenRequest = refreshTokenRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}

