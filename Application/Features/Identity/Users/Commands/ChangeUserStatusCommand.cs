using Common.Requests.Identity.Users;

namespace Application.Features.Identity.Users.Commands;

public class ChangeUserStatusCommand : IRequest<IResponseWrapper>
{
    public ChangeUserStatusRequest ChangeUserStatusRequest { get; set; }
}

public class ChangeUserStatusCommandHandler : IRequestHandler<ChangeUserStatusCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public ChangeUserStatusCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
    {
        return await _userService.ChangeUserStatusAsync(request.ChangeUserStatusRequest);
    }
}
