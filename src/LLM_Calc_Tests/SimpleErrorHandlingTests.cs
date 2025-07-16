using LLM_Calc;

namespace LLM_Calc_Tests;

[TestClass]
public class SimpleErrorHandlingTests
{
    [TestMethod]
    public void SimpleErrorHandlingTest()
    {
        var err = Assert.ThrowsException<ParserError>(() =>
        {
            var result = LlmCalculator.Calculate("2+", true);
        });

        Assert.AreEqual("Expected a valid expression at position 3 after token PLUS, but got EOL", err.Message);
    }

    [TestMethod]
    public void SimpleErrorHandlingTooManyDots()
    {
        var result = LlmCalculator.Calculate("2.2.2 + 1.1.1");

        Assert.AreEqual("ERROR: Unexpected token '.' in expression at position 4", result);
    }

    [TestMethod]
    public void SimpleErrorHandlingTest2()
    {
        var err = Assert.ThrowsException<ScannerError>(() =>
        {
            var result = LlmCalculator.Calculate(" [1]", true);
        });

        Assert.AreEqual("Unexpected character '[' at position 2", err.Message);
    }

    [TestMethod]
    public void SimpleErrorHandlingTest3()
    {
        var err = Assert.ThrowsException<ScannerError>(() =>
        {
            var result = LlmCalculator.Calculate(@"2
+
2", true);
        });

        Assert.AreEqual("Did not expect line-break in expression", err.Message);
    }

    [TestMethod]
    public void SimpleErrorHandling_NotImplementedYet()
    {
        var err = Assert.ThrowsException<ParserError>(() =>
        {
            var result = LlmCalculator.Calculate("test 2", true);
        });

        Assert.AreEqual("Unexpected constant or function 'test' found at position 1", err.Message);
    }

}