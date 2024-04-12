using MathGame.API.Entities.User;
using MathGame.Core.Entities.UserInSession;

namespace MathGame.Infrastructure.Context.Interface.Repositories;
public interface IUserInGameSessionRepository
{
    Task<IEnumerable<UserInGameSession>> GetUsersInGameSessionByGameSession(int gameSessionId);

    Task<User> GetUserIdByEmail(string userEmail);

    Task<UserInGameSession> GetUserInGameSessionByEmail(string email);

    Task<IEnumerable<Guid>> GetUsersInGameSessionUids(Guid gameSessionUid);

    void Insert(UserInGameSession entity);

    Task SaveAsync();
}
