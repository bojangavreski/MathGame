using MathGame.Core.Entities;
using MathGame.Infrastructure.Context.Interface;
using MathGame.Infrastructure.Repositories.Interface;

namespace MathGame.Infrastructure.Repositories.UserMathExpressionRepository;
public class UserMathExpressionRepository : Repository<UserMathExpression>, IUserMathExpressionRepository
{
    public UserMathExpressionRepository(IMathGameContext mathGameContext) : base(mathGameContext)
    {
    }
}
