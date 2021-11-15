using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace CetchUp.EquationElements
{
    internal static class EquationHelper
    {
        public static IEquationElement CreateEquationElementFromLine(string line, ref List<string> dependencies)
        {
            if (Regex.IsMatch(line, @"^\(.*\)$"))
            {
                return new EEequation(line.Substring(1, line.Length - 2), ref dependencies);
            }
            if (Regex.IsMatch(line, "^[A-z_]*$"))
            {
                return new EEvariable(line, ref dependencies);
            }
            if (Regex.IsMatch(line, @"^[0-9\.]*$"))
            {
                return new EEconstant(line);
            }
            if (Regex.IsMatch(line, @"^[+\-*\/]$"))
            {
                return new EEsymbol(line);
            }
            return new EEequation(line, ref dependencies);
        }

        ///<summary>
        ///Returns true when the <paramref name="element"> is either a variable or an equation thats a multiplicative of a variable
        ///<param name="element">The element that should be checked.</param>
        ///<param name="variableName">Receives the variable name of the variable element.</param>
        ///<param name="amount">Receives the multiplicative amount of the variable.</param>
        ///<returns>Returns if the element is a variable of multiplicative of a variable or not.</returns>
        ///</summary>
        public static bool IsElementVariableMultiplication(IEquationElement element, out string variableName, out float amount)
        {
            variableName = null;
            amount = 0;

            if (element is EEvariable)
            {
                EEvariable variableElement = (EEvariable)element;
                variableName = variableElement.name;
                amount = variableElement.isNegative ? -1 : 1;
                return true;
            }

            if (!(element is EEequation)) { return false; }
            EEequation equationElement = (EEequation)element;

            if (equationElement.elements.Count != 3) { return false; }
            if (!(equationElement.elements[1] is EEsymbol)) { return false; }
            if (!(equationElement.elements[0] is EEconstant || equationElement.elements[2] is EEconstant)) { return false; }
            
            if (equationElement.elements[0] is EEvariable)
            {
                GetVariableMultiplicationInfo((EEvariable)equationElement.elements[0],(EEconstant)equationElement.elements[2],ref variableName, ref amount);
                return true;
            }
            if (equationElement.elements[2] is EEvariable)
            {
                GetVariableMultiplicationInfo((EEvariable)equationElement.elements[2],(EEconstant)equationElement.elements[0],ref variableName, ref amount);
                return true;
            }
            return false;
        }

        private static void GetVariableMultiplicationInfo(EEvariable variableElement, EEconstant constantElement, ref string variableName, ref float amount)
        {
            variableName = variableElement.name;
            amount = constantElement.value;
            if (variableElement.isNegative)
            {
                amount *= -1;
            }
        }
    }
}