using System;
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

        public CalculateValue(CetchUpObject cetchUpObject)
        {
            currentValue = GetValueFromValueElement(equation[0]);

            EEmodifier lastMod;
            for (int i = 1; i < equation.Count; i++)
            {
                if (equation[i] is EEmodifier)
                {
                    lastMod = (EEmodifier)equation[i];
                }
                if (equation[i] is EEconstant || equation[i] is EEvariable)
                {
                    switch (lastMod.modtype)
                    {
                        case EEmodifier.ModifierType.Add: currentValue += GetValueFromValueElement(equation[i]); break;
                        case EEmodifier.ModifierType.Subtract: currentValue -= GetValueFromValueElement(equation[i]); break;
                        case EEmodifier.ModifierType.Multiply: currentValue *= GetValueFromValueElement(equation[i]); break;
                        case EEmodifier.ModifierType.Divide: currentValue /= GetValueFromValueElement(equation[i]); break;
                    }
                }
            }
        }

        private float GetValueFromValueElement(IEquationElement element)
        {
            if (element is EEconstant) { return ((EEconstant)equation[0]).constantValue; }
            if (element is EEvariable) { return cetchUpObject.GetValue(((EEvariable)equation[0]).variableName); }
            throw new ArgumentException("Expected a value");
        }
    }
}