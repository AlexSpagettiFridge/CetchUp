using CetchUp.CetchLines;
using static CetchUp.CetchValue;

namespace CetchUp
{
    public class ValueModEntry
    {
        public string ModName => origin.CetchModifier.Name;
        public string EquationString => equation.EquationString;

        internal EquationLine equation;
        public readonly CetchModifierEntry origin;
        public float value;
        internal ValuePart valuePart;

        internal ValueModEntry(EquationLine equation, CetchModifierEntry origin, float value, ValuePart valuePart)
        {
            this.equation = equation;
            this.origin = origin;
            this.value = value;
            this.valuePart = valuePart;
        }
    }
}