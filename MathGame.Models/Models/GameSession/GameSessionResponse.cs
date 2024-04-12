namespace MathGame.Models.Models.GameSession;
public class GameSessionResponse
{
    public Guid Uid { get; set; }

    public int? PositionInQueue { get; set; }

    public int ActivePlayersInGame {  get; set; }

    public int CurrentUserScore {  get; set; }
}
