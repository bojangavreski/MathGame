using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MathGame.Services.Interface.Services;

namespace MathGame.API.Hubs;

[Authorize]
public class MathGameHub : Hub
{
    private readonly ICacheService _cacheService;

    private readonly string HUB_CONNECTION_CACHE_KEY = "hubcachekey_";

    public MathGameHub(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public override async Task OnConnectedAsync()
    {
        ClaimsPrincipal claimsPrincipal = Context.User;
        _cacheService.Insert($"{HUB_CONNECTION_CACHE_KEY}{claimsPrincipal.Identity.Name}", Context.ConnectionId);
        base.OnConnectedAsync();
    }
}
