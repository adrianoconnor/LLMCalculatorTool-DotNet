namespace LLM_Calc.AST;

public class FunctionExpression(Token functionName, Expression? parameter) : Expression
{
    // Supported function names and number of parameters (arity)
    public static readonly Dictionary<string, int> AvailableFunctions = new()
    {
        { "sin", 1 },
        { "asin", 1 },
        { "sinh", 1 },
        { "asinh", 1 },
        { "cos", 1 },
        { "acos", 1 },
        { "cosh", 1 },
        { "acosh", 1 },
        { "tan", 1 },
        { "atan", 1 },
        { "tanh", 1 },
        { "atanh", 1 },
        { "sqrt", 1 },
    };

    public override decimal Evaluate()
    {
        return functionName.Lexeme switch
        {
            "sin" => (decimal)Math.Sin((double)parameter!.Evaluate()),
            "asin" => (decimal)Math.Asin((double)parameter!.Evaluate()),
            "tan" => (decimal)Math.Tan((double)parameter!.Evaluate()),
            "atan" => (decimal)Math.Atan((double)parameter!.Evaluate()),
            "cos" => (decimal)Math.Cos((double)parameter!.Evaluate()),
            "acos" => (decimal)Math.Acos((double)parameter!.Evaluate()),
            "sinh" => (decimal)Math.Sinh((double)parameter!.Evaluate()),
            "asinh" => (decimal)Math.Asinh((double)parameter!.Evaluate()),
            "tanh" => (decimal)Math.Tanh((double)parameter!.Evaluate()),
            "atanh" => (decimal)Math.Atanh((double)parameter!.Evaluate()),
            "cosh" => (decimal)Math.Cosh((double)parameter!.Evaluate()),
            "acosh" => (decimal)Math.Acosh((double)parameter!.Evaluate()),
            "sqrt" => (decimal)Math.Sqrt((double)parameter!.Evaluate()),
            _ => throw new NotImplementedException($"The function '{functionName.Lexeme}' is not implemented.")
        };
    }

    public override string ToString(int indent = 0)
    {
        return $"Function[name: {functionName}, expr: {parameter?.ToString()}]" + Environment.NewLine;
    }
}