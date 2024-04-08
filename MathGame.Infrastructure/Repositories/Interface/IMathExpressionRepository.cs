using MathGame.Core.Entities;

namespace MathGame.Infrastructure.Repositories.Interface;
public interface IMathExpressionRepository
{
    Task<MathExpression> Find(Guid uid);

    void Insert(MathExpression mathExpression);

    Task SaveAsync();
}
