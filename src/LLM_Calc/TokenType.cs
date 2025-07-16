namespace LLM_Calc;

public enum TokenType
{
    LEFT_BRACKET,
    RIGHT_BRACKET,

    COMMA,
    DOT,
    MINUS,
    PLUS,
    POW,

    DIVIDE,
    MULTIPLY,

    BITWISE_AND,
    BITWISE_OR,
    BITWISE_XOR,
    BITWISE_NOT,

    // Literals

    IDENTIFIER,
    NUMBER,

    EOL
}