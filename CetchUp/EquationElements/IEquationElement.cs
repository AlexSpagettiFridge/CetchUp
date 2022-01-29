using System;
using System.Collections.Generic;

namespace CetchUp.EquationElements
{
    internal interface IEquationElement : ICloneable
    {
        void Init(string line, ref List<string> dependencies);
        void Init(string line);
        float GetValue();
        float GetValue(CetchModifierEntry cetchModifierEntry);
        void Reroll();
    }
}