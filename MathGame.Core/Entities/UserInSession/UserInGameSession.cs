using System.Collections.ObjectModel;

namespace MathGame.Core.Entities.UserInSession;

public class UserInGameSession : TrackedTable
{
    public int Score { get; set; }

    public string UserFk { get; set; }

    public int GameSessionFk { get; set; }

    public virtual GameSession GameSession { get; set; }

    public virtual ICollection<MathExpression> MathExpressions { get; set; }

    public virtual ICollection<UserMathExpression> UserMathExpressions { get; set; }
}
