namespace MathGame.Core.Entities.UserInSession;

public class UserInGameSession : TrackedTable
{
    public int Score { get; set; }

    public string UserFk { get; set; }

    public int GameSessionFk { get; set; }

    public virtual GameSession GameSession { get; set; }
}
