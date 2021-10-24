using System;
using System.Globalization;
using CetchUp.EquationElements;

namespace CetchUp
{
    internal static class EquationHelper
    {
        internal static IEquationElement ParseValueElement(string snippet)
        {
            float number;
            if (float.TryParse(snippet, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out number))
            {
                return new EEconstant(number);
            }
            else
            {
                return new EEvariable(snippet);
            }
        }

        internal static float GetValueFromValueElement(CetchUpObject cetchUpObject, IEquationElement element)
        {
            if (element is EEconstant) { return ((EEconstant)element).constantValue; }
            if (element is EEvariable) { return cetchUpObject.GetValue(((EEvariable)element).variableName); }
            throw new ArgumentException("Expected a value");
        }
    }
}