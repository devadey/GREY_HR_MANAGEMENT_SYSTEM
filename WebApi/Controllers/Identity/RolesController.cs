﻿
using Application.Features.Identity.Roles.Commands;
using Application.Features.Identity.Roles.Queries;
using Common.Requests.Identity.Roles;

namespace WebApi.Controllers.Identity;

[Route("api/[controller]")]
public class RolesController : BaseController<RolesController>
{
    private readonly ISender _sender;

    public RolesController(ISender sender, ITokenService tokenService) : base(sender, tokenService)
    {
        this._sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleRequest request)
    {
        var response = await _sender.Send(new CreateRoleCommand { CreateRoleRequest = request });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var response = await _sender.Send(new GetRolesQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(string id)
    {
        var response = await _sender.Send(new GetRoleByIdQuery { RoleId = id });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole(UpdateRoleRequest request)
    {
        var response = await _sender.Send(new UpdateRoleCommand { UpdateRoleRequest = request});
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpDelete]
    [MustHavePermission(AppFeature.RoleClaims, AppAction.Delete)]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var response = await _sender.Send(new DeleteRoleCommand { RoleId = roleId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpGet("permissions/{roleId}")]
    [MustHavePermission(AppFeature.RoleClaims, AppAction.Read)]
    public async Task<IActionResult> GetPermissions(string roleId)
    {
        var response = await _sender.Send(new GetPermissionsQuery { RoleId = roleId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);

    }


    [HttpPut("update-permissions")]
    [MustHavePermission(AppFeature.RoleClaims, AppAction.Update)]
    public async Task<IActionResult> GetPermissions(UpdateRolePermissionRequest updateRolePermissionRequest)
    {
        var response = await _sender.Send(new UpdateRolesPermissionCommand { UpdateRolePermissionRequest = updateRolePermissionRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);

    }
}
