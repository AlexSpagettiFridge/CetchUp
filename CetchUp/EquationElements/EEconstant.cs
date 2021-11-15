using System.Globalization;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    internal struct EEconstant : IEquationElement
    {
        public float value;
        public bool isNegative;

        public EEconstant(string line)
        {
            isNegative = Regex.IsMatch(line, "^-.+$");
            if (isNegative) { line = line.Substring(1); }
            value = float.Parse(line, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        }

        public EEconstant(float value)
        {
            isNegative = false;
            this.value = value;
        }

        public float GetValue()
        {
            return value * (isNegative ? -1 : 1);
        }

        public float GetValue(CetchModifierEntry cetchModifierEntry)
        {
            return GetValue();
        }

        public IEquationElement Copy() => new EEconstant(value);

        public static EEconstant Zero => new EEconstant(0);

        public override string ToString()
        {
            return isNegative ? (-value).ToString() : value.ToString();
        }
    }
}