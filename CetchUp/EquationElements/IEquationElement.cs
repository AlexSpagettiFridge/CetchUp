using System;

namespace CetchUp.EquationElements
{
    internal interface IEquationElement : ICloneable
    {
        float GetValue();
        float GetValue(CetchModifierEntry cetchModifierEntry);
    }
}