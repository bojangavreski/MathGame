using MathGame.API.Hubs.Services;
using MathGame.Models.Models.GameSession;
using MathGame.Services.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathGame.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class GameSessionController : ControllerBase
{
    private readonly IGameSessionService _gameSessionService;
    private readonly IMathGameHubService _mathGameHubService;

    public GameSessionController(IGameSessionService gameSessionService,
                                 IMathGameHubService mathGameHubService)
    {
        _gameSessionService = gameSessionService;
        _mathGameHubService = mathGameHubService;
    }


    [HttpPost]
    [Route("join")]
     public async Task<GameSessionResponse> JoinGameSession()
    {
        var gameSessionResponse = await _gameSessionService.JoinGameSession();
        return gameSessionResponse;
    }

    [HttpPost]
    [Route("test")]
    public async Task TestHub()
    {
        await _mathGameHubService.NotifyUser("test");
    }

    [HttpPost]
    [Route("test2")]
    public string TestCors([FromBody] object something)
    {
        return "okay";
    }
}
