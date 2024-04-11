using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Identity
{

    [ApiController]
    public class BaseController<T>:ControllerBase
    {
        private ISender _sender;

        public ISender MediatorSender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
    }
}
