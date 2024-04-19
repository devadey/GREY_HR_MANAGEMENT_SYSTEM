using Common.Requests.Identity.Users;

namespace Application.Features.Identity.Users.Commands;

public class UpdateuserCommand : IRequest<IResponseWrapper>
{
    public UpdateUserRequest UpdateUserRequest { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateuserCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(UpdateuserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.UpdateUserAsync(request.UpdateUserRequest);
    }
}
