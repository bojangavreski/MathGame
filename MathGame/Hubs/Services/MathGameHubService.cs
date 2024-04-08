
using MathGame.Models.Models;
using MathGame.Models.Models.Notification;
using MathGame.Services.Interface.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace MathGame.API.Hubs.Services;
public class MathGameHubService : IMathGameHubService
{
    private readonly IHubContext<MathGameHub> _hubContext;
    private readonly IUserService _userService;
    private readonly ICacheService _cacheService;
    private readonly string HUB_CONNECTION_CACHE_KEY = "hubcachekey_";

    public MathGameHubService(IHubContext<MathGameHub> hubContext,
                              IUserService userService,
                              ICacheService cacheService)
    {
        _hubContext = hubContext;
        _userService = userService;
        _cacheService = cacheService;
    }

    public async Task NotifyUser(string email)
    {
        await _hubContext.Clients.All.SendAsync(email);
    }

    public async Task InsertCurrentUserInGameSessionGroup(string gameSessionUid)
    {
        var currentUserEmail = _userService.GetCurrentUserEmail();
        string userConnectionId = _cacheService.Get<string>($"{HUB_CONNECTION_CACHE_KEY}{currentUserEmail}");
        if (currentUserEmail != null)
        {
            await _hubContext.Groups.AddToGroupAsync(userConnectionId, gameSessionUid);
        }
    }

    public async Task NotifyAllPlayersMissed(string gameSessionUid,
                                             Guid mathExpressionUid)
    {

        PlayersMissedNotification playersMissedNotification = new PlayersMissedNotification
        {
            MathExpressionUid = mathExpressionUid
        };

        var request = JsonSerializer.Serialize(playersMissedNotification);
        var connectionIdToExclude = _cacheService.Get<string>($"{HUB_CONNECTION_CACHE_KEY}{_userService.GetCurrentUserEmail()}");

        await _hubContext.Clients.GroupExcept(gameSessionUid, connectionIdToExclude).SendAsync("NotifyMissed", request);
    }

    public async Task SendMathExpressionInGameSessionGroup(string gameSessionUid, MathExpressionResponse mathExpressionResponse)
    {
        var serializedMathExpressionResponse = JsonSerializer.Serialize(mathExpressionResponse);

        await _hubContext.Clients.Groups(gameSessionUid).SendAsync("GetMathExpression", serializedMathExpressionResponse);
    }
}
