namespace WebApi.Attributes;

public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string feature, string action) => AppPermission.NameFor(feature, action);
}
