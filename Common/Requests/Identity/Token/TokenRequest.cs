using Common.Responses.Wrappers;

namespace Common.Requests.Identity.Token;

public class TokenRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}



