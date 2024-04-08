using MathGame.Models.Models.GameSession;

namespace MathGame.Services.Interface.Services;
public interface IGameSessionService
{
    Task<GameSessionResponse> JoinGameSession();
}
