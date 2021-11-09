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
            line = Regex.Replace(line, @"^\(|\)$", "");
            string[] sides = Regex.Split(line, @"\(.*\)|[*\/+-]|-?[A-z_]+|-?[0-9\.]+");
            foreach (string match in sides)
            {
                IEquationElement equationElement = EquationHelper.CreateEquationElementFromLine(match, ref dependencies);
                elements.Add(equationElement);
                if (equationElement is EEvariable)
                {
                    dependencies.Add(((EEvariable)equationElement).name);
                }
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
                        ArrayList subEquation = elements.GetRange(i, 3);
                        elements[i] = new EEequation(subEquation);
                        elements.RemoveRange(i + 1, 2);
                    }
                }
            }
        }

        public EEequation(ArrayList elements)
        {
            this.elements = elements;
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
    }
}