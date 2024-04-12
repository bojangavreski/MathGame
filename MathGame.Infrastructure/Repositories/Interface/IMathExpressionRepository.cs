using MathGame.Core.Entities;

namespace MathGame.Infrastructure.Repositories.Interface;
public interface IMathExpressionRepository
{
    Task<bool> IsMathExpressionMisansweredByEveryUserInGameSession(Guid mathExpressionUid, IEnumerable<Guid> usersInGameSessionUids);

    Task<MathExpression> Find(Guid uid);

    void Insert(MathExpression mathExpression);

    Task SaveAsync();
}
