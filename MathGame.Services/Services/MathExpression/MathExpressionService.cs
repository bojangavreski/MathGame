using MathGame.API.Hubs.Services;
using MathGame.Core.Entities;
using MathGame.Core.Interfaces.Services;
using MathGame.Infrastructure.Context.Interface.Repositories;
using MathGame.Infrastructure.Repositories.Interface;
using MathGame.Models.Models;
using MathGame.Models.Models.MathExpression;
using MathGame.Services.Interface.Services;

namespace MathGame.Services.Services;
public class MathExpressionService : IMathExpressionService
{
    private readonly IMathGeneratorService _mathGeneratorService;
    private readonly IMathExpressionRepository _mathExpressionRepository;
    private readonly SemaphoreSlim _semaphore;
    private readonly IMathGameHubService _mathGameHubService;
    private readonly IUserInGameSessionService _userInGameSessionService;
    private readonly IUserInGameSessionRepository _userInGameSessionRepository;
    private readonly IUserMathExpressionRepository _userMathExpressionRepository;

    public MathExpressionService(IMathGeneratorService mathGeneratorService, 
                                 IMathExpressionRepository mathExpressionRepository,
                                 SemaphoreSlim semaphoreSlim,
                                 IMathGameHubService mathGameHubService,
                                 IUserInGameSessionService userInGameSessionService,
                                 IUserInGameSessionRepository userInGameSessionRepository,
                                 IUserMathExpressionRepository userMathExpressionRepository)
    {
        _mathGeneratorService = mathGeneratorService;
        _mathExpressionRepository = mathExpressionRepository;
        _semaphore = semaphoreSlim;
        _mathGameHubService = mathGameHubService;
        _userInGameSessionService = userInGameSessionService;
        _userInGameSessionRepository = userInGameSessionRepository;
        _userMathExpressionRepository = userMathExpressionRepository;
    }
    
    public async Task<AnswerExpressionResponse> AnswerExpression(AnswerExpressionRequest answerExpressionRequest)
    {
        await _semaphore.WaitAsync();

        bool shouldHandleCorrectAnswer = false;
        bool allParticipantsMistookTheExpression = false;

        MathExpression mathExpression = await _mathExpressionRepository.Find(answerExpressionRequest.ExpressionUid);
        AnswerExpressionResponse answerExpressionResponse = 
                            CreateAnswerExpressionResponse(answerExpressionRequest.ExpressionUid, MathExpressionResultOutcome.Wrong);
        Guid gameSessionUid = mathExpression.GameSession.Uid;

        try
        {
            if(mathExpression == null)
            {
                return answerExpressionResponse;
            }

            if(IsProvidedAnswerCorrect(answerExpressionRequest.ProvidedAnswer, mathExpression))
            {
                shouldHandleCorrectAnswer = true;
            }
            else
            {
                IEnumerable<Guid> userInGameSessionUids = await _userInGameSessionRepository.GetUsersInGameSessionUids(gameSessionUid);
                allParticipantsMistookTheExpression = 
                    await _mathExpressionRepository.IsMathExpressionMisansweredByEveryUserInGameSession(mathExpression.Uid, userInGameSessionUids);
                
                UserMathExpression dbUserMathExpression = new UserMathExpression
                {
                    Uid = Guid.NewGuid(),
                    UserInGameSessionFk = await _userInGameSessionService.GetCurrentUserId(),
                    MathExpressionFk = mathExpression.Id
                };

                _userMathExpressionRepository.Insert(dbUserMathExpression);
                return CreateAnswerExpressionResponse(mathExpression.Uid, MathExpressionResultOutcome.Wrong);
            }
        }
        finally{
            //Notify players here :D 
            MathExpressionResponse newMathExpression = null;
            if(shouldHandleCorrectAnswer)
            {
                newMathExpression = await GenerateNewMathExpressionAndDeleteCurrent(mathExpression);
                answerExpressionResponse = CreateAnswerExpressionResponse(mathExpression.Uid, MathExpressionResultOutcome.Correct);
                await HandleCorrectAnswer(gameSessionUid.ToString(), mathExpression.Uid);
            }

            if (allParticipantsMistookTheExpression)
            {
                newMathExpression = await GenerateNewMathExpressionAndDeleteCurrent(mathExpression);
            }
            
            await _mathExpressionRepository.SaveAsync();

            if(shouldHandleCorrectAnswer || allParticipantsMistookTheExpression)
            {
                await _mathGameHubService.SendMathExpressionInGameSessionGroup(gameSessionUid.ToString(), newMathExpression);
            }

            _semaphore.Release();
        }

        return answerExpressionResponse;
    }


    private async Task<MathExpressionResponse> GenerateNewMathExpressionAndDeleteCurrent(MathExpression mathExpression)
    {
        mathExpression.DeletedOn = DateTime.UtcNow;
        return await GetMathExpression(mathExpression.GameSession);
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
