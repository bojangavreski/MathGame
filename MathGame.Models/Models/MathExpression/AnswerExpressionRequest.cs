namespace MathGame.Models.Models.MathExpression;
public class AnswerExpressionRequest
{
    public Guid ExpressionUid {  get; set; }
    public bool ProvidedAnswer { get; set; }
}