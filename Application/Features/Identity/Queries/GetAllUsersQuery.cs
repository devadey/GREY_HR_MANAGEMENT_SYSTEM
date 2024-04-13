
namespace Application.Features.Identity.Queries;

public class GetAllUsersQuery : IRequest<IResponseWrapper>
{
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IResponseWrapper>
{
    private readonly IUserService _userService;

    public GetAllUsersQueryHandler(IUserService userService)
    {
        this._userService = userService;
    }
    public async Task<IResponseWrapper> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetAllUsersAsync();
    }
}
