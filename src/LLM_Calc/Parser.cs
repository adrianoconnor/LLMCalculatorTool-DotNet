using LLM_Calc.AST;

namespace LLM_Calc;

public class ParserError : Exception
{
    public ParserError(string message) : base(message)
    {

    }
}

public class Parser
{
    private readonly Token[] _tokens;
    private int _current = 0;

    public Parser(IList<Token> tokens)
    {
        this._tokens = tokens.ToArray();
    }

    public Expression Parse()
    {
        var expression = this.ParseExpression();

        if (!ReachedEnd())
        {
            throw new ParserError($"Unexpected token '{this.Peek().Lexeme}' in expression at position {this.Peek().StartPosition + 1}");
        }

        return expression;
    }

    private bool Match(params TokenType[] tokenTypes)
    {
        foreach (var tokenType in tokenTypes)
        {
            if (Check(tokenType))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private bool Check(TokenType tokenType, int skip = 0)
    {
        if (ReachedEnd()) return false;
        return Peek(skip).Type == tokenType;
    }

    private Token Peek(int skip = 0)
    {
        return _tokens[_current + skip];
    }

    private Token Advance()
    {
        if (!ReachedEnd()) _current++;

        return Previous();
    }

    public Token Previous(int skip = 0)
    {
        return _tokens[_current - 1 - (skip * 1)];
    }

    public Token Consume(TokenType tokenType, string errorIfNotFound)
    {
        if (Check(tokenType)) return Advance();

        throw new ParserError(errorIfNotFound);
    }

    private bool ReachedEnd()
    {
        return Peek().Type == TokenType.EOL;
    }


    private Expression ParseExpression()
    {
        var expr = Term();

        return expr;
    }

    private Expression Term()
    {
        var expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            var op = Previous();

            try
            {
                var right = Factor();
                expr = new BinaryExpression(expr, op, right);
            }
            catch (Exception)
            {
                throw new ParserError($"Expected a valid expression at position {_current + 1} after token {Previous().Type}, but got {Peek().Type}");
            }

        }

        return expr;
    }

    private Expression Factor()
    {
        var expr = Pow();

        while (Match(TokenType.MULTIPLY, TokenType.DIVIDE))
        {
            var op = Previous();
            var right = Pow();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression Pow()
    {
        var expr = Unary();

        while (Match(TokenType.POW))
        {
            var op = Previous();
            var right = Unary();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression Unary()
    {
        if (Match(TokenType.MINUS))
        {
            var op = Previous();
            var right = Unary();
            return new UnaryExpression(op, right);
        }

        return Primary();
    }

    private Expression Primary()
    {


        if (Match(TokenType.NUMBER))
        {
            return new LiteralExpression((Decimal)Previous().Literal!);
        }

        if (Match(TokenType.IDENTIFIER))
        {
            if (ConstantExpression.AvailableConstants.Contains(Previous().Lexeme))
            {
                return new ConstantExpression(Previous());
            }

            if (FunctionExpression.AvailableFunctions.ContainsKey(Previous().Lexeme))
            {
                var function = Previous();
                Expression? parameter = null;

                if (FunctionExpression.AvailableFunctions[Previous().Lexeme] == 1)
                {
                    parameter = ParseExpression();
                }

                return new FunctionExpression(function, parameter);
            }

            throw new ParserError($"Unexpected constant or function '{Previous().Lexeme}' found at position {Previous().StartPosition + 1}");
        }

        if (Match(TokenType.LEFT_BRACKET))
        {
            var expr = ParseExpression();

            Consume(TokenType.RIGHT_BRACKET, "Expect ')' after expression.");

            return new GroupingExpression(expr);
        }

        throw new ParserError($"Parser did not expect to see '{Peek().Lexeme}'");
    }
}