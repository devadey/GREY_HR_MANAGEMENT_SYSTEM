using Application.Services.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Identity
{

    [ApiController]
    public class BaseController<T>:ControllerBase
    {
        private ISender _sender;
        private readonly ITokenService _tokenService;

        public BaseController(ISender sender, ITokenService tokenService)
        {
            _sender = sender;
            this._tokenService = tokenService;
        }

        public ISender MediatorSender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
    }
}
