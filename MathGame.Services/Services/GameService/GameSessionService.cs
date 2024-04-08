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
        GameSessionResponse gameSessionResponse = CreateGameSessionResponse(gameSession.Uid);

        //TODO: throw exception if user already in game session
        IEnumerable<UserInGameSession> usersInGameSession = gameSession.UsersInGameSession.Where(x => x.DeletedOn == null);

        if(usersInGameSession.Count() < 5)
        {
            await _userInGameSessionService.InsertCurrentUserInGameSession(gameSession);
            
            if(usersInGameSession.Count() == 1)
            {
                await StartGameSession(gameSession);
            }

            //TODO: Provide unanswered expression to the newly joined player
        }
        else
        {
            _userInGameSessionService.EnqueueCurrentUser(gameSession.Id, out int? positionInQueue);
            gameSessionResponse.PositionInQueue = positionInQueue;
        }

        await _gameSessionRepository.SaveAsync();
        return gameSessionResponse;
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

    private GameSession CreateNewGameSession()
    {
        return new GameSession
        {
            Uid = Guid.NewGuid(),
            UsersInGameSession = new Collection<UserInGameSession>(),
            MathExpressions = new Collection<MathExpression>()
        };
    }

    private GameSessionResponse CreateGameSessionResponse(Guid gameSessionUid, int? positionInQueue = null)
    {
        return new GameSessionResponse
        {
            Uid = gameSessionUid
        };
    }
}
