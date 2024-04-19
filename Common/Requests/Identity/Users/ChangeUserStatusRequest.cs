namespace Common.Requests.Identity.Users;

public class ChangeUserStatusRequest
{
    public string UserId { get; set; }
    public bool UserStatus { get; set; }
}
