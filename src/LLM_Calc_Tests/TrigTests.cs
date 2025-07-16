using LLM_Calc;

namespace LLM_Calc_Tests;

[TestClass]
public class TrigTests
{
    [TestMethod]
    public void Sin45()
    {
        var result = LlmCalculator.Calculate("sin (PI/4)");
        Assert.AreEqual("0.707106781", result);
    }

    [TestMethod]
    public void ASin90()
    {
        var result = LlmCalculator.Calculate("asin(sin(PI/2))");
        Assert.AreEqual(((decimal)(Math.PI / 2)).ToString("0.#########"), result);
    }

    [TestMethod]
    public void Sin45_degrees_conversion()
    {
        var result = LlmCalculator.Calculate("sin (45 * (PI / 180))");
        Assert.AreEqual("0.707106781", result);

        // Need to re-evaluate how much we rely on decimal type to convert back again...
        // Maybe we need BigDecimal..?

        //var result2 = LlmCalculator.Calculate("asin (sin (45 * (PI / 180))) / (PI / 180)");
        //Assert.AreEqual("45", result2);
    }
}