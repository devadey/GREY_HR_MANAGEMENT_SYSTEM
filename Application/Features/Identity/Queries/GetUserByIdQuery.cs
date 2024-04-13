﻿
namespace Application.Features.Identity.Queries;

public class GetUserByIdQuery : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, IResponseWrapper>
{
    private readonly IUserService _userService;

    public GetUserByIdQueryHandler(IUserService userService)
    {
        this._userService = userService;
    }
    public async Task<IResponseWrapper> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByIdAsync(request.UserId);
    }
}
