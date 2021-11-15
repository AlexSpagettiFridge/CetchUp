namespace CetchUp.EquationElements
{
    internal interface IEquationElement
    {
        float GetValue();
        float GetValue(CetchModifierEntry cetchModifierEntry);

        IEquationElement Copy();
    }
}