using System;
using System.Globalization;
using System.Text.RegularExpressions;
using CetchUp.EquationElements;

namespace CetchUp
{
    internal static class EquationHelper
    {
        internal static IEquationElement ParseValueElement(string snippet, CetchModifier cetchModifier)
        {
            float number;
            if (float.TryParse(snippet, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out number))
            {
                return new EEconstant(number);
            }
            else
            {
                if (Regex.IsMatch(snippet, "^#:*"))
                {
                    return new EElocalVariable(snippet, cetchModifier);
                }
                return new EEvariable(snippet);
            }
        }

        internal static float GetValueFromValueElement(CetchModifierEntry cetchModifierEntry, IEquationElement element)
        {
            if (element is EEconstant) { return ((EEconstant)element).constantValue; }
            if (element is EEvariable) { return cetchModifierEntry.CetchUpObject.GetValue(((EEvariable)element).variableName); }
            if (element is EElocalVariable)
            {
                EElocalVariable localVariable = (EElocalVariable)element;
                return cetchModifierEntry.GetValue(localVariable.variableName);
            }
            throw new ArgumentException("Expected a value");
        }
    }
}