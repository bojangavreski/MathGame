using MathGame.Core.Enums;

namespace MathGame.Models.Models;
public class MathExpressionResponse
{
    public Guid ExpressionUid { get; set; }

    public string MathExpressionDisplay { get; set; }

    public QuestionType QuestionType { get; set; }
}
