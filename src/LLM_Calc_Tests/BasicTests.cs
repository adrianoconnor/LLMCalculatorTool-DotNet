using LLM_Calc;

namespace LLM_Calc_Tests;

[TestClass]
public class BasicTests
{
    [TestMethod]
    public void TestVerySimpleMaths_JustZero()
    {
        var result = LlmCalculator.Calculate("0");
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void TestVerySimpleMaths_JustZeroPointZero()
    {
        var result = LlmCalculator.Calculate("0.0");
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void TestVerySimpleMaths_JustZeroPointZeroes()
    {
        var result = LlmCalculator.Calculate("0.000+0.0000");
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void TestVerySimpleMaths_NoFloatingNonsense()
    {
        var result = LlmCalculator.Calculate("0.1 + 0.2");
        Assert.AreEqual("0.3", result);
    }

    [TestMethod]
    public void TestVerySimpleMaths_NoFloatingNonsenseNoTrailingZeros()
    {
        var result = LlmCalculator.Calculate("0.10 + 0.20");
        Assert.AreEqual("0.3", result);
    }

    [TestMethod]
    public void TestSimpleMaths()
    {
        var result1 = LlmCalculator.Calculate("2 * 3 + 4");
        Assert.AreEqual("10", result1);

        var result2 = LlmCalculator.Calculate("4 + 3 * 2");
        Assert.AreEqual("10", result2);

    }

    [TestMethod]
    public void TestPow()
    {
        var result = LlmCalculator.Calculate("2**2");
        Assert.AreEqual("4", result);
    }

    [TestMethod]
    public void TestNegative()
    {
        var result = LlmCalculator.Calculate("-(2+2)");
        Assert.AreEqual("-4", result);
    }

    [TestMethod]
    public void TestWhitespaceDoesNotMatter()
    {
        var result = LlmCalculator.Calculate(" (   2 +          2 )                 ");
        Assert.AreEqual("4", result);
    }

    [TestMethod]
    public void TestDivideAndReMultiply()
    {
        var result = LlmCalculator.Calculate("(100/3)*3");
        Assert.AreEqual("100", result);
    }
    
    [TestMethod]
    public void TestDivideAndReMultiplyBodmas()
    {
        var result = LlmCalculator.Calculate("100/3*3");
        Assert.AreEqual("100", result);
    }
}