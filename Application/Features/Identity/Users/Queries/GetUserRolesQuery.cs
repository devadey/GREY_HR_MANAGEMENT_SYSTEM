
namespace Application.Features.Identity.Users.Queries;

public class GetUserRolesQuery : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IResponseWrapper>
{
    private readonly IUserService _userService;

    public GetUserRolesQueryHandler(IUserService userService)
    {
        this._userService = userService;
    }
    public async Task<IResponseWrapper> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserRolesAsync(request.UserId);
    }
}
