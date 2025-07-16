using System.Diagnostics;

namespace LLM_Calc.AST;

public class ConstantExpression(Token constantName) : Expression
{
    public static readonly string[] AvailableConstants = { "PI" };

    public override decimal Evaluate()
    {
        return constantName.Lexeme switch
        {
            "PI" => (decimal)Math.PI,
            _ => throw new NotImplementedException($"The constant '{constantName.Lexeme}' is not known.")
        };
    }

    public override string ToString(int indent = 0)
    {
        return "Constant[name:" + constantName.ToString() + "]" + Environment.NewLine;
    }
}