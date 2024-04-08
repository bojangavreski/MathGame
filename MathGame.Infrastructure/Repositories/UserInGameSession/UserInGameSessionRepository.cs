using MathGame.API.Entities.User;
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


    public async Task<User> GetUserIdByEmail(string userEmail)
    {
        return await _mathGameContext.Users.SingleOrDefaultAsync(x => x.Email == userEmail);
    }

    public async Task<UserInGameSession> GetUserInGameSessionByEmail(string email)
    {
        return await (from dbUser in _mathGameContext.Users.Where(x => x.Email == email)
                      join dbUserInGameSession in All()
                      on dbUser.Id equals dbUserInGameSession.UserFk
                      select dbUserInGameSession).FirstOrDefaultAsync();
    }

}
