using System;
using System.Collections.Generic;
using System.Globalization;
using CetchUp.EquationElements;

namespace CetchUp
{
    internal class EquationLine : ICetchLine
    {
        private bool isMultiplier = false;
        private float currentValue = 0;
        private List<IEquationElement> equation = new List<IEquationElement>();
        private string modifiedValue;

        public float Value => currentValue;
        public bool IsMultiplier => isMultiplier;
        public event EventHandler<EquationLine> removed;

        public EquationLine(string cetchLine)
        {
            PopulateFromCetchLine(cetchLine);
        }

        public void JoinObject(CetchUpObject cetchUpObject)
        {
            cetchUpObject.GetCetchValue(modifiedValue).ModifyValue(this);
        }

        public void Remove()
        {
            if (removed != null) { removed.Invoke(this, this); }
        }

        private void PopulateFromCetchLine(string line, bool preShortened = false)
        {
            if (!preShortened)
            {
                line.Replace(" ", "");
            }
            int equalSymbolIndex = line.IndexOfAny(new char[] { '=', '%' });
            if (line.ToCharArray()[equalSymbolIndex] == '%')
            {
                isMultiplier = true;
            }
            modifiedValue = line.Substring(0, equalSymbolIndex);
            line = line.Substring(equalSymbolIndex + 1);

            bool loop = true;
            while (loop)
            {
                int nextSymbolIndex = line.IndexOfAny(new char[] { '+', '-', '/', '*', ';' });
                string frontArea = line.Substring(0, nextSymbolIndex);
                float number;
                if (float.TryParse(frontArea, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out number))
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

        public float CalculateValue(CetchUpObject cetchUpObject)
        {
            currentValue = GetValueFromValueElement(cetchUpObject, equation[0]);

            EEmodifier lastMod = new EEmodifier(EEmodifier.ModifierType.Add);
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
                        case EEmodifier.ModifierType.Add: currentValue += GetValueFromValueElement(cetchUpObject, equation[i]); break;
                        case EEmodifier.ModifierType.Subtract: currentValue -= GetValueFromValueElement(cetchUpObject, equation[i]); break;
                        case EEmodifier.ModifierType.Multiply: currentValue *= GetValueFromValueElement(cetchUpObject, equation[i]); break;
                        case EEmodifier.ModifierType.Divide: currentValue /= GetValueFromValueElement(cetchUpObject, equation[i]); break;
                    }
                }
            }

            return currentValue;
        }

        private float GetValueFromValueElement(CetchUpObject cetchUpObject, IEquationElement element)
        {
            if (element is EEconstant) { return ((EEconstant)equation[0]).constantValue; }
            if (element is EEvariable) { return cetchUpObject.GetValue(((EEvariable)equation[0]).variableName); }
            throw new ArgumentException("Expected a value");
        }

        public void OnVariableChanged(object sender, float newValue)
        {
            CetchValue cetchValue = (CetchValue)sender;
            cetchValue.ModifyValue(this);
        }
    }
}