using MathGame.Core.Entities.UserInSession;
namespace MathGame.Core.Entities;

public class GameSession : TrackedTable
{
    public virtual ICollection<UserInGameSession> UsersInGameSession { get; set; }

    public virtual ICollection<MathExpression> MathExpressions{ get; set; }

}
