namespace Common.Requests.Identity.Users;

public class UpdateUserRequest
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
}
