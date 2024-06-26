﻿using Common.Requests.Identity.Roles;
using Common.Requests.Identity.Users;

namespace WebApi.Controllers.Identity;

[Route("api/[controller]")]
public class UsersController : BaseController<UsersController>
{
    private readonly ISender _sender;
    private readonly ITokenService _tokenService;
    public UsersController(ISender sender, ITokenService tokenService) : base(sender, tokenService)
    {
        _sender = sender;
        _tokenService = tokenService;

    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest request)
    {
        var response = await _sender.Send(new UserRegistrationCommand { UserRegistrationRequest = request });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }


    [HttpGet("{userId}")]
    [MustHavePermission(AppFeature.Users, AppAction.Read)]
    public async Task<IActionResult> GetUserById([FromRoute] string userId)
    {
        var response = await _sender.Send(new GetUserByIdQuery { UserId = userId });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpGet]
    [MustHavePermission(AppFeature.Users, AppAction.Read)]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _sender.Send(new GetAllUsersQuery());

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPut]
    [MustHavePermission(AppFeature.Users, AppAction.Update)]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest updateUserRequest)
    {
        var response = await _sender.Send(new UpdateUserCommand { UpdateUserRequest = updateUserRequest });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPut("change-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangeUserPassword(ChangePasswordRequest changePasswordRequest)
    {
        var response = await _sender.Send(new ChangeUserPasswordCommand { ChangePasswordRequest = changePasswordRequest });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPut("change-status")]
    [MustHavePermission(AppFeature.Users, AppAction.Update)]
    public async Task<IActionResult> ChangeUserStatus(ChangeUserStatusRequest changeUserStatusRequest)
    {
        var response = await _sender.Send(new ChangeUserStatusCommand { ChangeUserStatusRequest = changeUserStatusRequest });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpGet("roles/{userId}")]
    [MustHavePermission(AppFeature.Roles, AppAction.Read)]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var response = await _sender.Send(new GetUserRolesQuery { UserId = userId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpPut("user-roles")]
    [MustHavePermission(AppFeature.Roles, AppAction.Update)]
    public async Task<IActionResult> UpdateUserRoles(UpdateUserRoleRequest updateUserRoleRequest)
    {
        var response = await _sender.Send(new UpdateUserRolesCommand { UpdateUserRoleRequest = updateUserRoleRequest });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}
