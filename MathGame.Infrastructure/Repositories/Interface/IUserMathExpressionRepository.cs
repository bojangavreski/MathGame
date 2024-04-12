using MathGame.Core.Entities;

namespace MathGame.Infrastructure.Repositories.Interface;
public interface IUserMathExpressionRepository
{
    void Insert(UserMathExpression userMathExpression);
}
