using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    internal class EEequation : IEquationElement
    {
        #region Head
        public ArrayList elements = new ArrayList();

        public EEequation(string line)
        {
            List<string> unnecessaryList = new List<string>();
            Init(line, ref unnecessaryList);
        }

        public EEequation(string line, ref List<string> dependencies)
        {
            Init(line, ref dependencies);
        }

        public EEequation(ArrayList elements)
        {
            this.elements = elements;
        }

        private void Init(string line, ref List<string> dependencies)
        {
            MatchCollection sides = Regex.Matches(line, @"(Round|Floor|Ceil)\((.*)\)|\(.*\)|[*\/+-]|-?#?[A-z_]+|-?[0-9\.]+");
            foreach (Match match in sides)
            {
                IEquationElement equationElement = EquationHelper.CreateEquationElementFromLine(match.Value, ref dependencies);
                elements.Add(equationElement);
            }
            int i = 0;
            if (elements.Count <= 3) { return; }
            while (i < elements.Count)
            {
                if (elements[i] is EEsymbol)
                {
                    if (((EEsymbol)elements[i]).symbol == '*' || ((EEsymbol)elements[i]).symbol == '/')
                    {
                        i--;
                        ArrayList subEquation = (ArrayList)elements.GetRange(i, 3).Clone();
                        elements[i] = new EEequation(subEquation);
                        elements.RemoveRange(i + 1, 2);
                    }
                }
                i++;
            }
        }
        #endregion

        #region GetInfo
        public float GetValue()
        {
            float total = 0;
            EEsymbol lastSymbol = new EEsymbol("+");
            foreach (IEquationElement element in elements)
            {
                if (element is EEsymbol)
                {
                    lastSymbol = (EEsymbol)element;
                    continue;
                }
                switch (lastSymbol.symbol)
                {
                    case '+': total += element.GetValue(); break;
                    case '-': total -= element.GetValue(); break;
                    case '/': total /= element.GetValue(); break;
                    default: total *= element.GetValue(); break;

                }
            }
            return total;
        }

        public float GetValue(CetchModifierEntry cme)
        {
            float total = 0;
            EEsymbol lastSymbol = new EEsymbol("+");
            foreach (IEquationElement element in elements)
            {
                if (element is EEsymbol)
                {
                    lastSymbol = (EEsymbol)element;
                    continue;
                }
                switch (lastSymbol.symbol)
                {
                    case '+': total += element.GetValue(cme); break;
                    case '-': total -= element.GetValue(cme); break;
                    case '/': total /= element.GetValue(cme); break;
                    default: total *= element.GetValue(cme); break;

                }
            }
            return total;
        }

        /// <summary>
        /// Gathers all variable names from this equation.
        /// </summary>
        /// <result>A list of variable names.</result>
        public List<string> GetAllDependencies()
        {
            List<string> result = new List<string>();
            foreach (IEquationElement element in elements)
            {
                if (element is EEvariable)
                {
                    result.Add(((EEvariable)element).name);
                }
                if (element is EEequation)
                {
                    result.AddRange(((EEequation)element).GetAllDependencies());
                }
            }
            return result;
        }

        public bool IsConstantOnly(out float value)
        {
            EEsymbol lastSymbol = new EEsymbol('+');
            value = 0;
            foreach (IEquationElement element in elements)
            {
                if (element is EEsymbol)
                {
                    lastSymbol = (EEsymbol)element;
                    continue;
                }
                if (element is EEconstant)
                {
                    EEconstant constantElement = (EEconstant)element;
                    switch (lastSymbol.symbol)
                    {
                        case '+': value += constantElement.value; continue;
                        case '-': value -= constantElement.value; continue;
                        case '*': value *= constantElement.value; continue;
                        case '/': value /= constantElement.value; continue;
                    }
                }
                if (element is EEequation)
                {
                    if (((EEequation)element).IsConstantOnly(out float constantValue))
                    {
                        value += constantValue;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }
        #endregion

        public object Clone()
        {
            ArrayList newEquationElements = new ArrayList();
            foreach (IEquationElement element in elements)
            {
                newEquationElements.Add(element.Clone());
            }
            return new EEequation(newEquationElements);
        }

        public static IEquationElement operator *(EEequation a, float b)
        {
            a.TryShorten();
            ArrayList elements = new ArrayList();
            if (EquationHelper.IsElementVariableMultiplication(a, out string variableName, out float amount))
            {
                elements.Add(EquationHelper.CreateElement(0, new Dictionary<string, float> { { variableName, amount * b } }));
            }
            else
            {
                foreach (IEquationElement element in a.elements)
                {
                    if (element is EEconstant)
                    {
                        elements.Add(new EEconstant(element.GetValue() * b));
                        continue;
                    }
                    if (element is EEvariable)
                    {
                        EEvariable variableElement = (EEvariable)element;
                        elements.Add(EquationHelper.CreateElement(0, new Dictionary<string, float> { { variableElement.name, b } }));
                        continue;
                    }
                    if (element is EEequation)
                    {
                        elements.Add(((EEequation)element) * b);
                        continue;
                    }
                    elements.Add(element);
                }
            }
            EEequation c = new EEequation(elements);
            c.TryShorten();
            return c;
        }

        /// <summary>
        /// Encapsulates every multiplication or division operation in an extra Equation.
        /// This way factor based operations will have priority over additions and subtractions.
        /// <summary>
        private void EncapsuleFactorBasedOperations()
        {
            if (elements.Count <= 3) { return; }
            for (int i = 1; i < elements.Count - 1; i++)
            {
                if (elements[i] is EEsymbol)
                {
                    EEsymbol symbol = (EEsymbol)elements[i];
                    if (symbol.IsFactorBasedOperation)
                    {
                        ArrayList newEquationElements = new ArrayList();
                        newEquationElements.Add(elements[i - 1]);
                        elements.RemoveRange(i, 2);
                    }
                }
            }
        }

        public void InsertVariable(string varName, float value)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] is EEequation)
                {
                    ((EEequation)elements[i]).InsertVariable(varName, value);
                }
                if (elements[i] is EEvariable)
                {
                    if (((EEvariable)elements[i]).name == varName)
                    {
                        elements[i] = new EEconstant(value);
                    }
                }
            }
        }

        public void TryShorten()
        {
            EncapsuleFactorBasedOperations();
            for (int n = 0; n < elements.Count; n++)
            {
                if (!(elements[n] is EEequation)) { continue; }
                EEequation equation = (EEequation)elements[n];
                equation.TryShorten();
                if (equation.elements.Count == 1)
                {
                    elements[n] = equation.elements[0];
                }
            }
            int groupStartIndex = 0;
            float totalConstant = 0;
            bool isNegative = false;
            ArrayList newElements = new ArrayList();
            Dictionary<string, float> variables = new Dictionary<string, float>();
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] is EEsymbol)
                {
                    if (((EEsymbol)elements[i]).IsFactorBasedOperation)
                    {
                        isNegative = false;
                        newElements.Add(EquationHelper.CreateElement(totalConstant, variables));
                        totalConstant = 0;
                        groupStartIndex = i + 1;
                        variables = new Dictionary<string, float>();
                        newElements.Add(((IEquationElement)elements[i]).Clone());
                        continue;
                    }
                    isNegative = ((EEsymbol)elements[i]).symbol == '-';
                    continue;
                }
                if (elements[i] is EEconstant)
                {
                    EEconstant constantElement = (EEconstant)elements[i];
                    if (!isNegative)
                    {
                        totalConstant += constantElement.value;
                    }
                    else
                    {
                        totalConstant -= constantElement.value;
                    }
                    continue;
                }
                if (elements[i] is EEequation)
                {
                    if (((EEequation)elements[i]).IsConstantOnly(out float constantValue))
                    {
                        if (isNegative) { constantValue *= -1; }
                        totalConstant += constantValue;
                        continue;
                    }
                }
                if (EquationHelper.IsElementVariableMultiplication((IEquationElement)elements[i], out string variableName, out float amount))
                {
                    if (!variables.ContainsKey(variableName))
                    {
                        variables.Add(variableName, 0);
                    }
                    if (isNegative) { amount *= -1; }
                    variables[variableName] += amount;
                    continue;
                }
                newElements.Add(((IEquationElement)elements[i]).Clone());
            }
            if (newElements.Count > 0 && !(newElements[newElements.Count - 1] is EEsymbol)) { newElements.Add(new EEsymbol('+')); }
            newElements.Add(EquationHelper.CreateElement(totalConstant, variables));
            elements = newElements;

            if (elements.Count == 1)
            {
                if (elements[0] is EEequation)
                {
                    elements = ((EEequation)elements[0]).elements;
                }
            }
            EncapsuleFactorBasedOperations();
        }

        public override string ToString()
        {
            string result = "";
            foreach (IEquationElement element in elements)
            {
                if (element is EEequation)
                {
                    EEequation subEquation = (EEequation)element;
                    if (subEquation.elements.Count == 3)
                    {
                        if (subEquation.elements[1] is EEsymbol)
                        {
                            EEsymbol symbol = (EEsymbol)subEquation.elements[1];
                            if (symbol.IsFactorBasedOperation)
                            {
                                result += element.ToString();
                                continue;
                            }
                        }
                    }
                    result += $"({element.ToString()})";
                    continue;
                }
                if (element is EEsymbol)
                {
                    result += $" {element.ToString()} ";
                    continue;
                }
                result += element.ToString();
            }
            return result;
        }
    }
}