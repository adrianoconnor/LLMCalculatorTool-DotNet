using System.Diagnostics;

namespace LLM_Calc.AST;

public class UnaryExpression(Token @operator, Expression rightExpression) : Expression
{
    public override decimal Evaluate()
    {
        if (@operator.Type == TokenType.MINUS)
        {
            return 0 - rightExpression.Evaluate();
        }

        throw new NotImplementedException();
    }

    public override string ToString(int indent = 0)
    {
        return "UnaryOp[op:" + @operator + ", expr:" + rightExpression.ToString() + "]" + Environment.NewLine;
    }
}