using MathGame.Core.Entities;
using MathGame.Infrastructure.Context.Interface;
using MathGame.Infrastructure.Repositories.Interface;

namespace MathGame.Infrastructure.Repositories;
public class MathExpressionRepository : Repository<MathExpression>, IMathExpressionRepository
{
    public MathExpressionRepository(IMathGameContext mathGameContext) : base(mathGameContext)
    {
    }
}
