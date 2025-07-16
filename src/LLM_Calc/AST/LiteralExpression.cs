namespace LLM_Calc.AST;

public class LiteralExpression(Decimal value) : Expression
{
    public override Decimal Evaluate()
    {
        return value;
    }

    public override string ToString(int indent = 0)
    {
        return "Literal[value: " + value + "]";
    }
}