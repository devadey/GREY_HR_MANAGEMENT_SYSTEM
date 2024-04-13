namespace WebApi.Controllers.Identity;

[Route("api/[controller]")]
public class UsersController : BaseController<UsersController>
{
    private readonly ISender _sender;
    private readonly ITokenService _tokenService;
    public UsersController(ISender sender, ITokenService tokenService) : base(sender, tokenService)
    {
        _sender = sender;
        _tokenService = tokenService;

    }


    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest request)
    {
        var response = await _sender.Send(new UserRegistrationCommand { UserRegistrationRequest = request });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById([FromRoute] string userId)
    {
        var response = await _sender.Send(new GetUserByIdQuery { UserId = userId });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _sender.Send(new GetAllUsersQuery());

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}
