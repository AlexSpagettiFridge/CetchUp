using System.Globalization;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    internal struct EEconstant : IEquationElement
    {
        public float value;

        public EEconstant(string line)
        {
            value = float.Parse(line, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        }

        public EEconstant(float value)
        {
            this.value = value;
        }

        public float GetValue()
        {
            return value;
        }

        public float GetValue(CetchModifierEntry cetchModifierEntry)
        {
            return GetValue();
        }

        public object Clone() => new EEconstant(value);

        public static EEconstant Zero => new EEconstant(0);

        public override string ToString()
        {
            return value.ToString();
        }
    }
}