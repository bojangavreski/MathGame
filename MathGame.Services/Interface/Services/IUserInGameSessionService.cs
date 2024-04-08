using MathGame.Core.Entities;
using MathGame.Core.Entities.UserInSession;
namespace MathGame.Services.Interface.Services;
public interface IUserInGameSessionService
{
    Task<IEnumerable<UserInGameSession>> GetUsersInGameSessionsByGameSessionId(int gameSessionId);

    Task InsertCurrentUserInGameSession(GameSession gameSession);

    void EnqueueCurrentUser(int gameSessionId, out int? positionInQueue);

    Task IncreaseCurrentUserScore();
}
