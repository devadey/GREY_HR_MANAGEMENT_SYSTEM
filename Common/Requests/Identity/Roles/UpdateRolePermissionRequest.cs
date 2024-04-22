using Common.Responses.Identity.Roles;

namespace Common.Requests.Identity.Roles;

public class UpdateRolePermissionRequest
{
    public string RoleId { get; set; }
    public List<RoleClaimViewModel> RolesClaims { get; set; }
}
