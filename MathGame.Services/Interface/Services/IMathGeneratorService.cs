using MathGame.Core.Entities;

namespace MathGame.Core.Interfaces.Services;
public interface IMathGeneratorService
{
    MathExpression GenerateMathExpression();
}
