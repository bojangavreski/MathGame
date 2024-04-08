using MathGame.Models.Models;

namespace MathGame.API.Hubs.Services;

public interface IMathGameHubService
{
    Task NotifyUser(string email);

    Task InsertCurrentUserInGameSessionGroup(string gameSessionUid);

    Task SendMathExpressionInGameSessionGroup(string gameSessionUid, MathExpressionResponse mathExpressionResponse);

    Task NotifyAllPlayersMissed(string gameSessionUid, Guid mathExpressionUid);
}
