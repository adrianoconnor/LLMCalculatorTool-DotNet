using LLM_Calc;

namespace LLM_Calc_Tests;

[TestClass]
public class ParserTests
{
    [TestMethod]
    public void TestParserNumbersAndBasicOps()
    {
        var scanner = new Scanner("1.1 + 2 * 3 - 4");
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var ast = parser.Parse();

        //Console.WriteLine(ast.ToString());

        Console.WriteLine(ast.Evaluate());
    }

    [TestMethod]
    public void TestAstDump1()
    {
        var scanner = new Scanner("(1.1 + 2) * (3 - 1)");
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var ast = parser.Parse();

        Console.WriteLine(ast.ToString());

        //Console.WriteLine(ast.Evaluate());
    }

    [TestMethod]
    public void TestAstDump2()
    {
        var scanner = new Scanner("1.1 + 2 * 3 - 1");
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var ast = parser.Parse();

        Console.WriteLine(ast.ToString());

        //Console.WriteLine(ast.Evaluate());
    }

}