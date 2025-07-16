namespace LLM_Calc;

public class LlmCalculator
{
    public static string Calculate(string expression, bool raiseExceptionOnError = false)
    {
        try
        {
            var scanner = new Scanner(expression);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            var result = ast.Evaluate();

            return result.ToString("0.#########");
        }
        catch (Exception e)
        {
            if (raiseExceptionOnError)
            {
                throw;
            }

            return $"ERROR: {e.Message}";
        }
    }
}