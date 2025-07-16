using System.Text;

namespace LLM_Calc;

public class ScannerError : Exception
{
    public ScannerError(string message) : base(message)
    {

    }
}

public class Scanner
{
    private readonly string _source;
    private readonly char[] _sourceRaw;

    private int _startOfToken = 0;
    private int _currentPos = 0;
    private int _previous = 0;

    private readonly IList<Token> _tokens = new List<Token>();

    public Scanner(string source)
    {
        this._source = source;
        this._sourceRaw = source.ToCharArray();
    }

    public IList<Token> ScanTokens()
    {
        while (!ReachedEnd())
        {
            _startOfToken = _currentPos;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOL, "", null, _currentPos, _currentPos,
            _currentPos));

        return _tokens;
    }

    private void ScanToken()
    {
        char c = NextChar();

        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_BRACKET); break;
            case ')': AddToken(TokenType.RIGHT_BRACKET); break;
            case '.': AddToken(TokenType.DOT); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case '*':
                if (MatchNext('*'))
                {
                    AddToken(TokenType.POW);
                }
                else
                {
                    AddToken(TokenType.MULTIPLY);
                }
                break;
            case '/':
                AddToken(TokenType.DIVIDE);
                break;
            case '&':
                AddToken(TokenType.BITWISE_AND);
                break;
            case '|':
                AddToken(TokenType.BITWISE_OR);
                break;

            case ' ':
                _previous = _currentPos;
                break;
            case '\r':
            case '\t':
                // Ignore whitespace
                break;

            case '\n':

                throw new ScannerError("Did not expect line-break in expression");

            default:
                if (CharIsDigit(c))
                {
                    ProcessNumber();
                }
                else if (CharIsAlpha(c))
                {
                    ProcessIdentifier();
                }
                else
                {
                    throw new ScannerError($"Unexpected character '{c}' at position {_currentPos}");
                }

                break;
        }
    }

    private void AddToken(TokenType tokenType, object? literal = null)
    {
        var lexeme = _source.Substring(_startOfToken, _currentPos - _startOfToken);

        _tokens.Add(new Token(tokenType, lexeme, literal, _startOfToken + 1,
            _previous, _currentPos));

        _previous = _currentPos;
    }

    private bool ReachedEnd()
    {
        return _currentPos >= _source.Length;
    }

    private char NextChar()
    {
        return _sourceRaw[_currentPos++];
    }

    private char Peek(int peekAhead = 0)
    {
        if (ReachedEnd()) return '\0';
        return _sourceRaw[_currentPos + peekAhead];
    }

    private bool MatchNext(char charToMatch)
    {
        if (Peek() == charToMatch)
        {
            _ = NextChar();
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CharIsDigit(char c)
    {
        return (c >= '0' && c <= '9');
    }

    private bool CharIsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    private bool CharIsAlphaNumeric(char c)
    {
        return CharIsAlpha(c) || CharIsDigit(c);
    }

    private void ProcessNumber()
    {
        bool hasConsumedDecimalPoint = false;

        while (true)
        {
            while (CharIsDigit(Peek())) _ = NextChar();

            if (Peek() == '.' && CharIsDigit(Peek(1)) && !hasConsumedDecimalPoint)
            {
                _ = NextChar(); // Consume the '.'

                hasConsumedDecimalPoint = true;
                continue;
            }

            if (Peek() == ',' && CharIsDigit(Peek(1)))
            {
                _ = NextChar(); // Consume the ','
                continue;
            }

            break;
        }

        var stringValue = _source.Substring(_startOfToken, _currentPos - _startOfToken).Replace(",", "");
        AddToken(TokenType.NUMBER, Decimal.Parse(stringValue));
    }

    private void ProcessIdentifier()
    {
        while (CharIsAlphaNumeric(Peek())) _ = NextChar();

        var stringValue = _source.Substring(_startOfToken, _currentPos - _startOfToken);

        AddToken(TokenType.IDENTIFIER);
    }
}