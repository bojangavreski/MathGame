using MathGame.API.Entities.User;
using MathGame.API.Hubs.Services;
using MathGame.Core.Entities;
using MathGame.Core.Entities.UserInSession;
using MathGame.Infrastructure.Context.Interface.Repositories;
using MathGame.Models.Models.UserInGameSession;
using MathGame.Services.Interface.Services;

namespace MathGame.Services.Services;
public class UserInGameSessionService : IUserInGameSessionService
{
    private readonly IUserInGameSessionRepository _userInGameSessionRepository;
    private readonly IUserService _userService;
    private readonly ICacheService _cacheService;
    private readonly IMathGameHubService _mathGameHubService;
    private string GAME_SESSION_CACHE_KEY = $"game_session_queue_key=";

    public UserInGameSessionService(IUserInGameSessionRepository userInGameSessionRepository,
                                    IUserService userService,
                                    ICacheService cacheService,
                                    IMathGameHubService mathGameHubService)
    {
        _userInGameSessionRepository = userInGameSessionRepository;
        _userService = userService;
        _cacheService = cacheService;
        _mathGameHubService = mathGameHubService;
    }

    public async Task<IEnumerable<UserInGameSession>> GetUsersInGameSessionsByGameSessionId(int gameSessionId)
    {
        return await _userInGameSessionRepository.GetUsersInGameSessionByGameSession(gameSessionId);
    }

    public async Task InsertCurrentUserInGameSession(GameSession gameSession)
    {
        User user = await _userInGameSessionRepository.GetUserIdByEmail(_userService.GetCurrentUserEmail());

        if(gameSession.UsersInGameSession.Any(x => x.UserFk == user.Id))
        {
            return;
        }

        UserInGameSession userInGameSession = new UserInGameSession
        {
            Uid = Guid.NewGuid(),
            UserFk = user.Id,
            Score = default,
        };

        await _mathGameHubService.InsertCurrentUserInGameSessionGroup(gameSession.Uid.ToString());
        _userInGameSessionRepository.Insert(userInGameSession);
        gameSession.UsersInGameSession.Add(userInGameSession);
    }


    public void EnqueueCurrentUser(int gameSessionId, out int? positionInQueue)
    {
        positionInQueue = null;
        string cacheKey = $"{GAME_SESSION_CACHE_KEY}{gameSessionId}";
        EnqueuedUser userToEnqueue = CreateEnquedUser();
        SortedSet<EnqueuedUser> usersQueue = _cacheService.Get<SortedSet<EnqueuedUser>>(cacheKey);

        if (usersQueue == null)
        {
            usersQueue = new SortedSet<EnqueuedUser>();
        }
        else
        {
            bool userAlreadyEnqueued = usersQueue.Any(x => x.UserEmail == userToEnqueue.UserEmail);
            if (userAlreadyEnqueued)
            {
                positionInQueue = usersQueue.Count;
                return;
            }
        }

        positionInQueue = usersQueue.Count + 1;
        usersQueue.Add(userToEnqueue);
        _cacheService.Insert(cacheKey, usersQueue);
    }

    public async Task IncreaseCurrentUserScore()
    {
        UserInGameSession userInGameSesion =  await _userInGameSessionRepository.GetUserInGameSessionByEmail(_userService.GetCurrentUserEmail());
        userInGameSesion.Score += 1;
    }

    public async Task<int> GetCurrentUserId()
    {
        string currentUserEmail = _userService.GetCurrentUserEmail();
        UserInGameSession currentUserInGameSession = await _userInGameSessionRepository.GetUserInGameSessionByEmail(currentUserEmail);
        if(currentUserInGameSession != null)
        {
            return currentUserInGameSession.Id;
        }
        else
        {
            throw new Exception($"UserInGameSession {currentUserEmail} not found");
        }
    }

    private EnqueuedUser CreateEnquedUser()
    {
        return new EnqueuedUser
        {
            UserEmail = _userService.GetCurrentUser().Identity.Name,
            InsertedAt = DateTime.Now,
        };
    }
}
