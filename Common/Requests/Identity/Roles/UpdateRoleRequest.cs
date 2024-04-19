namespace Common.Requests.Identity.Roles;

public class UpdateRoleRequest
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public string RoleDescription { get; set; }
}
