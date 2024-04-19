using Common.Requests.Identity.Users;

namespace Application.Features.Identity.Users.Commands;

public class ChangeUserPasswordCommand : IRequest<IResponseWrapper>
{
    public ChangePasswordRequest ChangePasswordRequest { get; set; }
}

public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public ChangeUserPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _userService.ChangeUserPasswordAsync(request.ChangePasswordRequest);
    }
}
