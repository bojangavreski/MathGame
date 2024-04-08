using MathGame.Services.Interface.Services;
using System.Security.Claims;

namespace MathGame.Services.Services;
public class UserService : IUserService
{
    private readonly ClaimsPrincipal _currentUser;

    public UserService(ClaimsPrincipal currentUser)
    {
        _currentUser = currentUser;
    }

    public ClaimsPrincipal GetCurrentUser()
    {
        return _currentUser;
    }

    public string GetCurrentUserEmail()
    {
        return _currentUser.Identity.Name;
    }
}
