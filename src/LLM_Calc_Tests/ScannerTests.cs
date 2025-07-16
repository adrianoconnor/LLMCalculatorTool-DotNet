using System.Text;
using LLM_Calc;

namespace LLM_Calc_Tests;

[TestClass]
public class ScannerTests
{
    [TestMethod]
    public void TestScannerNumbersAndBasicOps()
    {
        var scanner = new Scanner("1.1 + 2 * 3 - 4");
        var result = scanner.ScanTokens();
        Assert.AreEqual(8, result.Count());

        Assert.IsTrue(result[0].Type == TokenType.NUMBER);
        Assert.AreEqual(1.1M, result[0].Literal);
    }

    [TestMethod]
    public void TestScannerNumbersAndBasicOpsNoSpaces()
    {
        var scanner = new Scanner("1.1+2*3-4");
        var result = scanner.ScanTokens();
        Assert.AreEqual(8, result.Count());

        Assert.IsTrue(result[0].Type == TokenType.NUMBER);
        Assert.AreEqual(1.1M, result[0].Literal);
    }

    [TestMethod]
    public void DumpTokens()
    {
        var scanner = new Scanner("1.1+2*(3-4**2)");
        var result = scanner.ScanTokens();

        var output = new StringBuilder();

        foreach (var token in result)
        {
            //Console.WriteLine(token);
            output.AppendLine(token.ToString());
        }

        Assert.AreEqual(@"Token: NUMBER (Lexeme = '1.1', literal value = 1.1)
Token: PLUS (Lexeme = '+', literal value = )
Token: NUMBER (Lexeme = '2', literal value = 2)
Token: MULTIPLY (Lexeme = '*', literal value = )
Token: LEFT_BRACKET (Lexeme = '(', literal value = )
Token: NUMBER (Lexeme = '3', literal value = 3)
Token: MINUS (Lexeme = '-', literal value = )
Token: NUMBER (Lexeme = '4', literal value = 4)
Token: POW (Lexeme = '**', literal value = )
Token: NUMBER (Lexeme = '2', literal value = 2)
Token: RIGHT_BRACKET (Lexeme = ')', literal value = )
Token: EOL (Lexeme = '', literal value = )
", output.ToString());
    }
}