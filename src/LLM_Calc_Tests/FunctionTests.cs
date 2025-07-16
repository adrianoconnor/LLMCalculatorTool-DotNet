using LLM_Calc;

namespace LLM_Calc_Tests;

[TestClass]
public class FunctionTests
{
    [TestMethod]
    public void TestSin()
    {
        var result = LlmCalculator.Calculate("sin (3.14159 / 2)");
        Assert.AreEqual("1", result);

        var scanner = new Scanner("sin (3.14159 / 2)");
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
            //output.AppendLine(token.ToString());
        }

        var parser = new Parser(tokens);
        var ast = parser.Parse();

        Console.WriteLine(ast.ToString());

        Console.WriteLine(ast.Evaluate());
    }
}