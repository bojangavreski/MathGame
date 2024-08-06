using MathGame.API.Hubs.Services;
using MathGame.Core.Entities;
using MathGame.Core.Entities.UserInSession;
using MathGame.Core.Interfaces.Services;
using MathGame.Infrastructure.Context.Interface.Repositories;
using MathGame.Models.Models;
using MathGame.Models.Models.GameSession;
using MathGame.Services.Interface.Services;
using System.Collections.ObjectModel;

namespace MathGame.Services.Services.GameService;
public class GameSessionService : IGameSessionService
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IUserInGameSessionService _userInGameSessionService;
    private readonly IMathGameHubService _mathGameHubService;
    private readonly IMathExpressionService _mathExpressionService;

    public GameSessionService(IGameSessionRepository gameSessionRepository,
                              IUserInGameSessionService userInGameSessionService,
                              IMathGameHubService mathGameHubService,
                              IMathExpressionService mathExpressionService)
    {
        _gameSessionRepository = gameSessionRepository;
        _userInGameSessionService = userInGameSessionService;
        _mathGameHubService = mathGameHubService;
        _mathExpressionService = mathExpressionService;
    }

    public async Task<GameSessionResponse> JoinGameSession()
    { 
        GameSession gameSession = await GetGameSession();
        IEnumerable<UserInGameSession> usersInGameSession = gameSession.UsersInGameSession.Where(x => x.DeletedOn == null);

        int? positionInQueue = null;

        if(usersInGameSession.Count() < 5)
        {
            IEnumerable<MathExpression> activeMathExpressionsInGameSession = gameSession.MathExpressions.Where(x => x.DeletedOn == null).ToList();

            await _userInGameSessionService.InsertCurrentUserInGameSession(gameSession);
            
            if(ShouldStartNewGameSession(usersInGameSession, activeMathExpressionsInGameSession))
            {
                await StartGameSession(gameSession);
            }
            else
            {
                await SendActiveMathExpressionToNewlyJoinedPlayer(activeMathExpressionsInGameSession, gameSession.Uid);
            }
        }
        else
        {
            _userInGameSessionService.EnqueueCurrentUser(gameSession.Id, out positionInQueue);
        }

        await _gameSessionRepository.SaveAsync();
        return CreateGameSessionResponse(gameSession.Uid, positionInQueue, default, gameSession.UsersInGameSession.Count());
    }

    private static bool ShouldStartNewGameSession(IEnumerable<UserInGameSession> usersInGameSession, IEnumerable<MathExpression> activeMathExpressionsInGameSession)
    {
        return usersInGameSession.Count() == 1 && !activeMathExpressionsInGameSession.Any();
    }

    private async Task SendActiveMathExpressionToNewlyJoinedPlayer(IEnumerable<MathExpression> activeMathExpressionsInGameSession, Guid gameSessionUid)
    {
        MathExpression? activeMathExpression = activeMathExpressionsInGameSession.FirstOrDefault();
        if(activeMathExpression != null) 
        {
            await _mathGameHubService.SendMathExpressionInGameSessionGroup(gameSessionUid.ToString(), CreateMathExpressionResponse(activeMathExpression));
        }
    }

    private static MathExpressionResponse CreateMathExpressionResponse(MathExpression mathExpression)
    {
        return new MathExpressionResponse
        {
            MathExpressionDisplay = $"{mathExpression.DisplayedExpression}={mathExpression.DisplayedResult}",
            ExpressionUid = mathExpression.Uid,
            QuestionType = mathExpression.QuestionType
        };
    }


    private async Task StartGameSession(GameSession gameSession)
    {
       MathExpressionResponse mathExpressionResponse =  await _mathExpressionService.GetMathExpression(gameSession);
       await _mathGameHubService.SendMathExpressionInGameSessionGroup(gameSession.Uid.ToString(), mathExpressionResponse);
    }

    
    private async Task<GameSession> GetGameSession()
    {
        GameSession gameSession = await _gameSessionRepository.GetActiveGameSession();

        if(gameSession == null)
        {
            gameSession = CreateNewGameSession();
            _gameSessionRepository.Insert(gameSession);
        }
        
        return gameSession;
    }

    private static GameSession CreateNewGameSession()
    {
        return new GameSession
        {
            Uid = Guid.NewGuid(),
            UsersInGameSession = new Collection<UserInGameSession>(),
            MathExpressions = new Collection<MathExpression>()
        };
    }

    private static GameSessionResponse CreateGameSessionResponse(Guid gameSessionUid,
                                                          int? positionInQueue,
                                                          int currentUserScore,
                                                          int activePlayersInGame)
    {
        return new GameSessionResponse
        {
            Uid = gameSessionUid,
            PositionInQueue = positionInQueue,
            CurrentUserScore = currentUserScore,
            ActivePlayersInGame = activePlayersInGame
        };
    }
}
