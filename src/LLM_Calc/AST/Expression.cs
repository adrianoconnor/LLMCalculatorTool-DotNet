namespace LLM_Calc.AST;

public abstract class Expression
{
    public abstract Decimal Evaluate();
    public abstract string ToString(int indent = 0);

    public override string ToString()
    {
        return ToString(0);
    }
}