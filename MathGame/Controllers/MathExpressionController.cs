using MathGame.Core.Interfaces.Services;
using MathGame.Models.Models.MathExpression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathGame.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MathExpressionController : ControllerBase
{
    private readonly IMathExpressionService _mathExpressionService;

    public MathExpressionController(IMathExpressionService mathExpressionService)
    {
        _mathExpressionService = mathExpressionService;
    }

    [HttpPost]
    [Route("answer")]
     public async Task<AnswerExpressionResponse> AnswerExpression([FromBody] AnswerExpressionRequest answerExpressionRequest)
    {
        var gameSessionResponse = await _mathExpressionService.AnswerExpression(answerExpressionRequest);
        return gameSessionResponse;
    }
}
