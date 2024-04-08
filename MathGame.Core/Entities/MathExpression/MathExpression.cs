using MathGame.Core.Enums;

namespace MathGame.Core.Entities;
public class MathExpression : TrackedTable
{
    public decimal ActualResult { get; set; }

    public decimal? DisplayedResult { get; set; }

    public string DisplayedExpression { get; set; }

    public QuestionType QuestionType { get; set; }

    public int GameSessionFk { get; set; }

    public virtual GameSession GameSession {  get; set; } 
}
