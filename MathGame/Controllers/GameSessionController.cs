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

    public GameSessionController(IGameSessionService gameSessionService)
    {
        _gameSessionService = gameSessionService;
    }


    [HttpPost]
    [Route("join")]
     public async Task<GameSessionResponse> JoinGameSession()
    {
        var gameSessionResponse = await _gameSessionService.JoinGameSession();
        return gameSessionResponse;
    }
}
