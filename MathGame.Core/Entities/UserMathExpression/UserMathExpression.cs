using MathGame.Core.Entities.UserInSession;

namespace MathGame.Core.Entities;

public class UserMathExpression : TrackedTable
{
    public int UserInGameSessionFk { get; set; }

    public int MathExpressionFk { get; set; }

    public virtual UserInGameSession UserInGameSession { get; set; }

    public virtual MathExpression MathExpression { get; set; }

}
