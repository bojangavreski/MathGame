using MathGame.Core.Entities;
using MathGame.Models.Models;
using MathGame.Models.Models.MathExpression;

namespace MathGame.Core.Interfaces.Services;
public interface IMathExpressionService
{
    Task<MathExpressionResponse> GetMathExpression(GameSession gameSession);

    Task<AnswerExpressionResponse> AnswerExpression(AnswerExpressionRequest answerExpressionRequest);
}
