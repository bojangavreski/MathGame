using System.Security.Claims;

namespace MathGame.Services.Interface.Services;
public interface IUserService
{
    ClaimsPrincipal GetCurrentUser();

    string GetCurrentUserEmail();
}
