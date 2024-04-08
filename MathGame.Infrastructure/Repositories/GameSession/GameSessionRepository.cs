using MathGame.Core.Entities;
using MathGame.Infrastructure.Context.Interface;
using MathGame.Infrastructure.Context.Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MathGame.Infrastructure.Repositories;
public class GameSessionRepository : Repository<GameSession>, IGameSessionRepository
{
    public GameSessionRepository(IMathGameContext mathGameContext) : base(mathGameContext)
    {
    }

    public async Task<GameSession> GetGameSession(int id, ClaimsPrincipal user)
    {
        var gameSession = await _mathGameContext.GameSessions.FirstOrDefaultAsync();

        return gameSession;
    }

    public async Task<GameSession> GetActiveGameSession()
    {
        return await _dbSet.AsTracking().SingleOrDefaultAsync(x => x.DeletedOn == null);
    }
}
