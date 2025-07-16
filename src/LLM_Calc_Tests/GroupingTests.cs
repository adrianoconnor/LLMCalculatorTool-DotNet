using LLM_Calc;

namespace LLM_Calc_Tests;

[TestClass]
public class GroupingTests
{
    [TestMethod]
    public void TestSimpleBrackets()
    {
        var result = LlmCalculator.Calculate("2 * (3 + 4)");
        Assert.AreEqual("14", result);
    }

    [TestMethod]
    public void TestMultipleNestedBrackets()
    {
        var result = LlmCalculator.Calculate("3 * (12 - (4 + 2))");
        Assert.AreEqual("18", result);
    }

    [TestMethod]
    public void TestMultipleNestedBrackets2()
    {
        var result = LlmCalculator.Calculate("3 * ((12 - (((4 + 2)))))");
        Assert.AreEqual("18", result);
    }
}