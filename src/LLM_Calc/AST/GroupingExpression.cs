namespace LLM_Calc.AST;

public class GroupingExpression(Expression groupedExpression) : Expression
{
    public override Decimal Evaluate()
    {
        return groupedExpression.Evaluate();
    }

    public override string ToString(int indent = 0)
    {
        var padding = "".PadLeft(indent + 2);

        return @$"Grouping[
{padding}expr: {groupedExpression.ToString(indent + 8)}
{padding}]";
    }
}