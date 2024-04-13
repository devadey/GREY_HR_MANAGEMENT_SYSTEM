namespace WebApi.Controllers;

[Authorize]
[ApiController]
public class BaseController<T> : ControllerBase
{
    private ISender _sender;
    private readonly ITokenService _tokenService;

    public BaseController(ISender sender, ITokenService tokenService)
    {
        _sender = sender;
        _tokenService = tokenService;
    }

    public ISender MediatorSender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
}
