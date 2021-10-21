using System.Collections.Generic;
using CetchUp.EquationElements;

namespace CetchUp
{
    internal struct ModedValue
    {
        private float currentValue = 0;
        private List<IEquationElement> equation = new List<IEquationElement>();
        private string modifiedValue;

        public void AddFromCetchLine(string line, bool preShortened = false)
        {
            if (!preShortened)
            {
                line.Replace(" ", "");
            }
            int equalSymbolIndex = line.IndexOf('=');
            modifiedValue = line.Substring(0, nextSymbolIndex);
            line = line.Substring(equalSymbolIndex + 1);

            bool loop = true;
            while (loop)
            {
                int nextSymbolIndex = line.IndexOfAny(new char[] { '+', '-', '/', '*', ';' });
                string frontArea = line.Substring(0, nextSymbolIndex);
                float number;
                if (float.TryParse(frontArea, out number))
                {
                    equation.Add(new EEconstant(number));
                }
                else
                {
                    equation.Add(new EEvariable(frontArea));
                }
                char currentSymbol = line.ToCharArray()[nextSymbolIndex];
                switch (currentSymbol)
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                        equation.Add(new EEmodifier(currentSymbol));
                        break;
                    case ';':
                        loop = false;
                        break;
                }
                line = line.Substring(nextSymbolIndex + 1);
            }
        }
    }
}