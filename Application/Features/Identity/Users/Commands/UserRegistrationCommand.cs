using Common.Requests.Identity.Users;

namespace Application.Features.Identity.Users.Commands;

public class UserRegistrationCommand : IRequest<IResponseWrapper>
{
    public UserRegistrationRequest UserRegistrationRequest { get; set; }
}

public class UserRegistrationCommandHandler(IUserService userService) : IRequestHandler<UserRegistrationCommand, IResponseWrapper>
{
    private readonly IUserService _userService = userService;

    public Task<IResponseWrapper> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
    {
        return _userService.ResgisterUserAsync(request.UserRegistrationRequest);
    }
}
