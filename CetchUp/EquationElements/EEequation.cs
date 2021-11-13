using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    internal class EEequation : IEquationElement
    {
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


        private void Init(string line, ref List<string> dependencies)
        {
            MatchCollection sides = Regex.Matches(line, @"\(.*\)|[*\/+-]|-?[A-z_]+|-?[0-9\.]+");
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

        public EEequation(ArrayList elements)
        {
            this.elements = elements;
        }

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

        public void ModifyByValue(float modify)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] is EEequation)
                {
                    ((EEequation)elements[i]).ModifyByValue(modify);
                }
                if (elements[i] is EEconstant)
                {
                    elements[i] = new EEconstant(((EEconstant)elements[i]).value * modify);
                    continue;
                }
                if (elements[i] is EEvariable)
                {
                    string varName = ((EEvariable)elements[i]).name;
                    elements[i] = new EEequation($"{varName}*{modify}");
                    continue;
                }
            }
            TryShorten();
        }

        public void TryShorten()
        {
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
            int i = 0;
            while (i < elements.Count - 2)
            {
                if (elements[i] is EEconstant)
                {
                    if (elements[i + 1] is EEsymbol && elements[i + 2] is EEconstant)
                    {
                        EEequation newEquation = new EEequation($"{elements[i].ToString()}{elements[i + 1].ToString()}{elements[i + 2].ToString()}");
                        EEconstant newElement = new EEconstant(newEquation.GetValue());
                        elements.RemoveRange(i + 1, 2);
                        if (newElement.GetValue() == 0)
                        {
                            elements.RemoveAt(i);
                            continue;
                        }
                        elements[i] = newElement;
                        continue;
                    }
                }
                if (elements[i] is EEvariable)
                {
                    if (elements[i + 1] is EEsymbol && elements[i + 2] is EEvariable)
                    {
                        EEsymbol symbol = (EEsymbol)elements[i + 1];
                        EEvariable var1 = (EEvariable)elements[i];
                        EEvariable var2 = (EEvariable)elements[i + 2];
                        if (var1.name == var2.name && (symbol.symbol == '+' || symbol.symbol == '-'))
                        {

                            int totalSign = (var1.isNegative ? -1 : 1);
                            if (symbol.symbol == '+')
                            {
                                totalSign += (var2.isNegative ? -1 : 1);
                            }
                            else
                            {
                                totalSign -= (var2.isNegative ? -1 : 1);
                            }
                            elements.RemoveRange(i + 1, 2);
                            if (totalSign == 0)
                            {
                                elements.RemoveAt(i);
                                continue;
                            }
                            elements[i] = new EEequation($"{var1.name}*{totalSign}");
                        }
                    }
                }
                i++;
            }
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
                            if (symbol.symbol == '*' || symbol.symbol == '/')
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