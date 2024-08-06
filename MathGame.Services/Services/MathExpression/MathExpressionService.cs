using MathGame.API.Hubs.Services;
using MathGame.Core.Entities;
using MathGame.Core.Interfaces.Services;
using MathGame.Infrastructure.Context.Interface.Repositories;
using MathGame.Infrastructure.Repositories.Interface;
using MathGame.Models.Models;
using MathGame.Models.Models.MathExpression;
using MathGame.Services.Interface.Services;
using System.Xml.XPath;

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

        MathExpression mathExpression = await _mathExpressionRepository.Find(answerExpressionRequest.ExpressionUid);
        MathExpressionResponse newMathExpression = null;

        Guid gameSessionUid = mathExpression.GameSession.Uid;

        try
        {
            if(mathExpression == null)
            {
                throw new KeyNotFoundException($"Expression with Uid: {answerExpressionRequest.ExpressionUid} was not found");
            }

            if(IsProvidedAnswerCorrect(answerExpressionRequest.ProvidedAnswer, mathExpression))
            {
                newMathExpression = await GenerateNewMathExpressionAndDeleteCurrent(mathExpression);
                return await HandleCorrectAnswer(gameSessionUid.ToString(), mathExpression.Uid);
            }
            else
            {
                IEnumerable<Guid> usersInGameSessionUids = await _userInGameSessionRepository.GetUsersInGameSessionUids(gameSessionUid);
                bool allParticipantsMistookTheExpression = await _mathExpressionRepository.IsMathExpressionMisansweredByEveryUserInGameSession(mathExpression.Uid, usersInGameSessionUids);
                if (allParticipantsMistookTheExpression)
                {
                    newMathExpression = await GenerateNewMathExpressionAndDeleteCurrent(mathExpression);
                }

                return await HandleIncorrectAnswer(mathExpression);
            }
        }
        finally{
            await _mathExpressionRepository.SaveAsync();

            if(newMathExpression != null)
            {
                await _mathGameHubService.SendMathExpressionInGameSessionGroup(gameSessionUid.ToString(), newMathExpression);
            }

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

    private async Task<AnswerExpressionResponse> HandleIncorrectAnswer(MathExpression mathExpression)
    {
        UserMathExpression dbUserMathExpression = new UserMathExpression
        {
            Uid = Guid.NewGuid(),
            UserInGameSessionFk = await _userInGameSessionService.GetCurrentUserId(),
            MathExpressionFk = mathExpression.Id
        };

        _userMathExpressionRepository.Insert(dbUserMathExpression);

        return CreateAnswerExpressionResponse(mathExpression.Uid, MathExpressionResultOutcome.Wrong);
    }

    private async Task<MathExpressionResponse> GenerateNewMathExpressionAndDeleteCurrent(MathExpression mathExpression)
    {
        mathExpression.DeletedOn = DateTime.UtcNow;
        return await GetMathExpression(mathExpression.GameSession);
    }

    private async Task<AnswerExpressionResponse> HandleCorrectAnswer(string gameSessionUid, Guid mathExpressionUid)
    {
        // TODO: This should be changed to exclude all of the players that have answered
        await _mathGameHubService.NotifyAllPlayersMissed(gameSessionUid.ToString(), mathExpressionUid);
        await _userInGameSessionService.IncreaseCurrentUserScore();
        return CreateAnswerExpressionResponse(mathExpressionUid, MathExpressionResultOutcome.Correct);
    }

    private AnswerExpressionResponse CreateAnswerExpressionResponse(Guid expressionUid, MathExpressionResultOutcome outcome)
    {
        return new AnswerExpressionResponse
        {
            ExpressionUid = expressionUid,
            MathExpressionResultOutcome = outcome
        };
    }

    private static bool IsProvidedAnswerCorrect(bool providedAnswer, MathExpression mathExpression)
    {
        bool isExpressionDisplayResultCorrect = mathExpression.ActualResult == mathExpression.DisplayedResult;

        return isExpressionDisplayResultCorrect == providedAnswer;
    }
}
