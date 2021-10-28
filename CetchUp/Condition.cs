using CetchUp.EquationElements;

namespace CetchUp
{
    internal class Condition
    {
        public IEquationElement firstValue, secondValue;
        public EEcomparison comparer;

        public Condition(string line, CetchModifier cetchModifier)
        {
            int compIndex = line.IndexOfAny(new char[] { '=', '<', '>' });
            firstValue = EquationHelper.ParseValueElement(line.Substring(0, compIndex), cetchModifier);
            line = line.Substring(compIndex);
            int compLenght = line.ToCharArray()[1] == '=' ? 2 : 1;
            comparer = new EEcomparison(line.Substring(0, compLenght));
            secondValue = EquationHelper.ParseValueElement(line.Substring(compLenght), cetchModifier);
        }

        public bool IsConditionMet(CetchModifierEntry cetchModifierEntry)
        {
            float a = EquationHelper.GetValueFromValueElement(cetchModifierEntry, firstValue);
            float b = EquationHelper.GetValueFromValueElement(cetchModifierEntry, secondValue);
            switch (comparer.comparisonType)
            {

                case EEcomparison.ComparisonType.greater:
                    if (comparer.isInclusive)
                    {
                        return a >= b;
                    }
                    return a > b;
                case EEcomparison.ComparisonType.smaller:
                    if (comparer.isInclusive)
                    {
                        return a <= b;
                    }
                    return a > b;
                default:
                    return a == b;
            }
        }
    }
}