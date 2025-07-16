namespace LLM_Calc.AST;

public class BinaryExpression(Expression leftExpression, Token binaryOperator, Expression rightExpression)
    : Expression
{
    public override Decimal Evaluate()
    {
        switch (binaryOperator.Type)
        {
            case TokenType.PLUS:
                return leftExpression.Evaluate() + rightExpression.Evaluate();
            case TokenType.MINUS:
                return leftExpression.Evaluate() - rightExpression.Evaluate();
            case TokenType.DIVIDE:
                return leftExpression.Evaluate() / rightExpression.Evaluate();
            case TokenType.MULTIPLY:
                return leftExpression.Evaluate() * rightExpression.Evaluate();
            case TokenType.POW:
                // TODO: This will overflow for large numbers, but right now we're just using Decimal everywhere
                return (decimal)Math.Pow((double)leftExpression.Evaluate(), (double)rightExpression.Evaluate());
            default:
                throw new NotImplementedException();
        }
    }

    public override string ToString(int indent = 0)
    {
        var outerPadding = "".PadLeft(indent);
        var innerPadding = "".PadLeft(indent + 2);

        return @$"BinaryOp[
{innerPadding} left: {leftExpression.ToString(indent + 9)},
{innerPadding}   op: {binaryOperator.ToString()},
{innerPadding}right: {rightExpression.ToString(indent + 9)}
{outerPadding}]";
    }
}