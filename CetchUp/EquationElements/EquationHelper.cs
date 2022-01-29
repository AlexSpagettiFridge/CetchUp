using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Collections;

namespace CetchUp.EquationElements
{
    internal static class EquationHelper
    {
        /// <summary>
        /// A collection of Expression strings needed to identify the different 
        /// <see cref="CetchUp.EquationElements.IEquationElement"/>.
        /// Also a nice alliteration
        /// </summary>
        /// <value></value>
        private static Dictionary<Type, string> EquationElementExpressions = new Dictionary<Type, string>{
            {typeof(EEfunction), @"(Round|Floor|Ceil)\(.*\)"},
            {typeof(EEequation), @"\(.*\)"},
            {typeof(EEsymbol), @"[+\-*\/]"},
            {typeof(EEroll), "[0-9]+d[0-9]+"},
            {typeof(EEvariable), @"-?(\@[0-9]*\.)?#?[A-z_]+"},
            {typeof(EEconstant), @"[0-9\.]+"},
        };

        public static string CombinedEquationElementExpression
        {
            get
            {
                string expression = "";
                int i = 0;
                foreach (string eee in EquationElementExpressions.Values)
                {
                    expression += eee;
                    i++;
                    if (i < EquationElementExpressions.Count)
                    {
                        expression += "|";
                    }
                }
                return expression;
            }
        }

        public static IEquationElement CreateEquationElementFromLine(string line)
        {
            List<string> unnecesaryDependencies = new List<string>();
            return CreateEquationElementFromLine(line, ref unnecesaryDependencies);
        }

        public static IEquationElement CreateEquationElementFromLine(string line, ref List<string> dependencies)
        {
            foreach (KeyValuePair<Type, string> entry in EquationElementExpressions)
            {
                if (Regex.IsMatch(line, $"^{entry.Value}$"))
                {
                    IEquationElement ee = (IEquationElement)entry.Key.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    ee.Init(line, ref dependencies);
                    return ee;
                }
            }
            EEequation equation = new EEequation();
            equation.Init(line, ref dependencies);
            return equation;
        }

        ///<summary>
        ///Returns true when the <paramref name="element"> is either a variable or an equation thats a multiplicative of a variable
        ///<param name="element">The element that should be checked.</param>
        ///<param name="variableName">Receives the variable name of the variable element.</param>
        ///<param name="amount">Receives the multiplicative amount of the variable.</param>
        ///<returns>Returns if the element is a variable of multiplicative of a variable or not.</returns>
        ///</summary>
        public static bool IsElementVariableMultiplication(IEquationElement element, out string variableName, out float amount, out int referenceId)
        {
            variableName = null;
            amount = 0;
            referenceId = -1;

            if (element is EEvariable)
            {
                EEvariable variableElement = (EEvariable)element;
                variableName = variableElement.name;
                amount = variableElement.isNegative ? -1 : 1;
                referenceId = variableElement.referenceId;
                return true;
            }

            if (!(element is EEequation)) { return false; }
            EEequation equationElement = (EEequation)element;

            if (equationElement.elements.Count != 3) { return false; }
            if (!(equationElement.elements[1] is EEsymbol)) { return false; }
            if (!(equationElement.elements[0] is EEconstant || equationElement.elements[2] is EEconstant)) { return false; }

            if (equationElement.elements[0] is EEvariable)
            {
                GetVariableMultiplicationInfo((EEvariable)equationElement.elements[0], (EEconstant)equationElement.elements[2], ref variableName, ref amount);
                return true;
            }
            if (equationElement.elements[2] is EEvariable)
            {
                GetVariableMultiplicationInfo((EEvariable)equationElement.elements[2], (EEconstant)equationElement.elements[0], ref variableName, ref amount);
                return true;
            }
            return false;
        }

        public static bool IsElementVariableMultiplication(IEquationElement element)
        {
            return IsElementVariableMultiplication(element, out string variableName, out float amount, out int referenceId);
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

        public static IEquationElement CreateElement(float totalConstant, Dictionary<KeyValuePair<string, int>, float> variables)
        {
            EEequation newQuation = new EEequation(new ArrayList());
            if (totalConstant != 0)
            {
                newQuation.elements.Add(new EEconstant(totalConstant));
            }
            foreach (KeyValuePair<KeyValuePair<string, int>, float> entry in variables)
            {
                if (entry.Value == 0) { continue; }
                if (newQuation.elements.Count != 0)
                {
                    newQuation.elements.Add(new EEsymbol(entry.Value >= 0 ? '+' : '-'));
                }
                EEvariable variableElement = new EEvariable(entry.Key.Key, false, entry.Key.Value);
                if (Math.Abs(entry.Value) == 1)
                {
                    newQuation.elements.Add(variableElement);
                    continue;
                }
                ArrayList subElements = new ArrayList();
                subElements.Add(variableElement);
                subElements.Add(new EEsymbol('*'));
                subElements.Add(new EEconstant(Math.Abs(entry.Value)));
                newQuation.elements.Add(new EEequation(subElements));
            }
            if (newQuation.elements.Count == 1)
            {
                return (IEquationElement)newQuation.elements[0];
            }

            return newQuation;
        }
    }
}