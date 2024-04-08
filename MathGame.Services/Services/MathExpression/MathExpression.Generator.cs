using MathGame.Core.Entities;
using MathGame.Core.Interfaces.Services;
using MathGame.Infrastructure.Repositories.Interface;
using System.Data;

namespace MathGame.Services.Services;
public class MathExpressionGenerator : IMathGeneratorService
{
    public MathExpression GenerateMathExpression()
    {
        DataTable dataTable = new DataTable();

        int firstOperand = GenerateRandomNumber();
        int secondOperand = GenerateRandomNumber();
        char mathOperation = GenerateRandomMathOperation();
        string mathExpression = $"{firstOperand}{mathOperation}{secondOperand}";
        decimal actualResult = Convert.ToDecimal(dataTable.Compute(mathExpression, ""));
        decimal displayResult = GetDisplayResult(actualResult);

        return CreateMathExpression(displayResult, actualResult, mathExpression);
    }

    private MathExpression CreateMathExpression(decimal displayResult,
                                                 decimal actualResult, string displayedMathExpression)
    {
        return new MathExpression
        {
            Uid = Guid.NewGuid(),
            ActualResult = actualResult,
            DisplayedResult = displayResult,
            DisplayedExpression = displayedMathExpression,
            QuestionType = Core.Enums.QuestionType.BinaryQuestion,
            CreatedOn = DateTime.UtcNow
        };
    }

    private int GenerateRandomNumber()
    {
        Random rnd = new Random();
        return rnd.Next(1, 11);
    }

    private bool GenerateRandomBoolean()
    {
        Random rnd = new Random();
        return rnd.Next(0, 2) == 1;
    }
    private char GenerateRandomMathOperation()
    {
        Random rnd = new Random();
        int randomNumber = rnd.Next(0, 4);
        switch (randomNumber)
        {
            case 0:
                return '+';
            case 1:
                return '-';
            case 2:
                return '*';
            default:
                return '/';
        }
    }

    private decimal GetDisplayResult(decimal actualResult)
    {
        Random rnd = new Random();
        bool showActualResult = GenerateRandomBoolean();

        if (showActualResult)
        {
            return actualResult;
        }

        decimal resultWithOffset = actualResult + rnd.Next(-5, 4) + 1;

        return resultWithOffset;
    }
}
