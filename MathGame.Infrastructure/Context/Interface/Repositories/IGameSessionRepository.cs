using MathGame.Core.Entities;
using System.Security.Claims;

namespace MathGame.Infrastructure.Context.Interface.Repositories;
public interface IGameSessionRepository
{
    public Task<GameSession> Find(int gameSessionId);

    Task<GameSession> GetGameSession(int id, ClaimsPrincipal user);

    Task<GameSession> GetActiveGameSession();

    void Insert(GameSession gameSession);

    Task SaveAsync();
}
