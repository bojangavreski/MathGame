using Castle.DynamicProxy;
using MathGame.API.Entities.User;
using MathGame.Core.Entities;
using MathGame.Core.Entities.UserInSession;
using MathGame.Infrastructure.Context.Interface;
using MathGame.Infrastructure.Context.Interface.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MathGame.Infrastructure.Repositories;
public class UserInGameSessionRepository : Repository<UserInGameSession>, IUserInGameSessionRepository
{
    public UserInGameSessionRepository(IMathGameContext mathGameContext) : base(mathGameContext)
    {
    }

    public async Task<IEnumerable<UserInGameSession>> GetUsersInGameSessionByGameSession(int gameSessionId)
    {
        return await _dbSet.Where(x => x.DeletedOn == null && x.GameSessionFk == gameSessionId)
                           .ToListAsync();
    }
    public async Task<UserInGameSession> GetUserInGameSessionByEmail(string email)
    {
        return await (from dbUser in _mathGameContext.Users.Where(x => x.UserName == email)
                      join dbUserInGameSession in AllNoTrackedOf<UserInGameSession>()
                      on dbUser.Id equals dbUserInGameSession.UserFk
                      select dbUserInGameSession).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserIdByEmail(string userEmail)
    {
        return await _mathGameContext.Users.SingleOrDefaultAsync(x => x.Email == userEmail);
    }

    public async Task<IEnumerable<Guid>> GetUsersInGameSessionUids(Guid gameSessionUid)
    {
        return await (from dbGameSession in AllNoTrackedOf<GameSession>().Where(x => x.Uid == gameSessionUid)
                      join dbUserInGameSession in AllNoTrackedOf<UserInGameSession>()
                      on dbGameSession.Id equals dbUserInGameSession.GameSessionFk
                      select dbUserInGameSession.Uid).ToListAsync();
    } 
}
