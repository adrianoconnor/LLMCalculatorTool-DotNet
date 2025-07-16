namespace LLM_Calc;

public class Token(TokenType type, string lexeme, object? literal, int col, int startPosition, int endPosition)
{
    public TokenType Type { get; private set; } = type;
    public string Lexeme { get; private set; } = lexeme;
    public object? Literal { get; private set; } = literal;

    public int Col { get; private set; } = col;
    public int StartPosition { get; private set; } = startPosition;
    public int EndPosition { get; private set; } = endPosition;

    public override string ToString()
    {
        return $"Token: {Type} (Lexeme = '{Lexeme}', literal value = {Literal})";
    }
}