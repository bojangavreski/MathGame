using MathGame.API.Hubs.Services;
using MathGame.Core.Entities;
using MathGame.Core.Interfaces.Services;
using MathGame.Infrastructure.Repositories.Interface;
using MathGame.Models.Models;
using MathGame.Models.Models.MathExpression;
using MathGame.Services.Interface.Services;
using System.Runtime.InteropServices;
using System.Xml.XPath;

namespace MathGame.Services.Services;
public class MathExpressionService : IMathExpressionService
{
    private readonly IMathGeneratorService _mathGeneratorService;
    private readonly IMathExpressionRepository _mathExpressionRepository;
    private readonly SemaphoreSlim _semaphore;
    private readonly IMathGameHubService _mathGameHubService;
    private readonly IUserInGameSessionService _userInGameSessionService;

    public MathExpressionService(IMathGeneratorService mathGeneratorService, 
                                 IMathExpressionRepository mathExpressionRepository,
                                 SemaphoreSlim semaphoreSlim,
                                 IMathGameHubService mathGameHubService,
                                 IUserInGameSessionService userInGameSessionService)
    {
        _mathGeneratorService = mathGeneratorService;
        _mathExpressionRepository = mathExpressionRepository;
        _semaphore = semaphoreSlim;
        _mathGameHubService = mathGameHubService;
        _userInGameSessionService = userInGameSessionService;
    }
    
    public async Task<AnswerExpressionResponse> AnswerExpression(AnswerExpressionRequest answerExpressionRequest)
    {
        await _semaphore.WaitAsync();

        MathExpression mathExpression = await _mathExpressionRepository.Find(answerExpressionRequest.ExpressionUid);
        try
        {
            if(mathExpression == null)
            {
                return CreateAnswerExpressionResponse(answerExpressionRequest.ExpressionUid, MathExpressionResultOutcome.Wrong);
            }

            string gameSessionUid = mathExpression.GameSession.Uid.ToString();

            if(IsProvidedAnswerCorrect(answerExpressionRequest.ProvidedAnswer, mathExpression))
            {
                //TODO: Do not notify players too early 
                await HandleCorrectAnswer(gameSessionUid, mathExpression.Uid);
                MathExpressionResponse newMathExpression = await GetMathExpression(mathExpression.GameSession);
                mathExpression.DeletedOn = DateTime.UtcNow;
                await _mathExpressionRepository.SaveAsync();
                await _mathGameHubService.SendMathExpressionInGameSessionGroup(gameSessionUid, newMathExpression);
                return CreateAnswerExpressionResponse(mathExpression.Uid, MathExpressionResultOutcome.Correct);
            }
            else
            {
                //TODO: if all players answer wrongfully - generate new math expression
                return CreateAnswerExpressionResponse(mathExpression.Uid, MathExpressionResultOutcome.Wrong);
            }
        }
        finally{
            _semaphore.Release();
        }
    }



    public async Task<MathExpressionResponse> GetMathExpression(GameSession gameSession)
    {
        MathExpression mathExpression = _mathGeneratorService.GenerateMathExpression();
        mathExpression.GameSession = gameSession;
        gameSession.MathExpressions.Add(mathExpression);

        return new MathExpressionResponse
        {
            MathExpressionDisplay = $"{mathExpression.DisplayedExpression}={mathExpression.DisplayedResult}",
            ExpressionUid = mathExpression.Uid,
            QuestionType = mathExpression.QuestionType
        };
    }

    private async Task HandleCorrectAnswer(string gameSessionUid, Guid mathExpressionUid)
    {
        // TODO: This should be changed to exclude all of the players that have answered
        await _mathGameHubService.NotifyAllPlayersMissed(gameSessionUid.ToString(), mathExpressionUid);
        await _userInGameSessionService.IncreaseCurrentUserScore();
    }

    private AnswerExpressionResponse CreateAnswerExpressionResponse(Guid expressionUid, MathExpressionResultOutcome outcome)
    {
        return new AnswerExpressionResponse
        {
            ExpressionUid = expressionUid,
            MathExpressionResultOutcome = outcome
        };
    }

    private bool IsProvidedAnswerCorrect(bool providedAnswer, MathExpression mathExpression)
    {
        bool correctExpressionResult = mathExpression.ActualResult == mathExpression.DisplayedResult;

        // XAND 
        return !(providedAnswer ^ correctExpressionResult);
    }
}
