using Common.Responses.Identity.Users;

namespace Common.Requests.Identity.Roles;

public class UpdateUserRoleRequest
{
    public string UserId { get; set; }
    public List<UserRoleViewModel> Roles { get; set; }
}
