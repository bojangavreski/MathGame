using MathGame.Core.Entities;
using MathGame.Core.Entities.UserInSession;
using MathGame.Infrastructure.Context.Interface;
using MathGame.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MathGame.Infrastructure.Repositories;
public class MathExpressionRepository : Repository<MathExpression>, IMathExpressionRepository
{
    public MathExpressionRepository(IMathGameContext mathGameContext) : base(mathGameContext)
    {
    }

    public async Task<bool> IsMathExpressionMisansweredByEveryUserInGameSession(Guid mathExpressionUid, IEnumerable<Guid> usersInGameSessionUids)
    {
        IEnumerable<UserInGameSession> usersInGameSessionThatAnswered = 
                                                await (from dbMathExpression in AllNoTrackedOf().Where(x => x.Uid == mathExpressionUid)
                                                        join dbUserMathExpression in AllNoTrackedOf<UserMathExpression>()
                                                        on dbMathExpression.Id equals dbUserMathExpression.MathExpressionFk
                                                        join dbUserInGameSession in AllNoTrackedOf<UserInGameSession>().Where(x => usersInGameSessionUids.Contains(x.Uid))
                                                        on dbUserMathExpression.UserInGameSessionFk equals dbUserInGameSession.Id
                                                        select dbUserInGameSession).ToListAsync();

        return usersInGameSessionThatAnswered.Count() == (usersInGameSessionUids.Count() - 1);
    }
}
