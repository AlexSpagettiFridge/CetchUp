using System;
using System.Collections.Generic;
using System.Globalization;
using CetchUp.EquationElements;

namespace CetchUp
{
    internal class EquationLine : ICetchLine
    {
        private bool isMultiplier = false;
        private List<IEquationElement> equation = new List<IEquationElement>();
        private string modifiedValue;

        public bool IsMultiplier => isMultiplier;
        public event EventHandler<EquationLine> removed;

        public EquationLine(string cetchLine)
        {
            PopulateFromCetchLine(cetchLine);
        }

        public void JoinObject(CetchUpObject cetchUpObject)
        {
            CetchValue cetchValue = cetchUpObject.GetCetchValue(modifiedValue);
            cetchValue.AddModedValue(this);
            cetchValue.ModifyValue(this);
            foreach (CetchValue depVariable in GetDependentValues(cetchUpObject))
            {
                depVariable.changed += OnVariableChanged;
            }
        }

        public void Remove(CetchUpObject cetchUpObject)
        {
            foreach (CetchValue depVariable in GetDependentValues(cetchUpObject))
            {
                depVariable.changed -= OnVariableChanged;
            }
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
                int nextSymbolIndex = line.IndexOfAny(new char[] { '+', '-', '/', '*', ';', '(', ')' });
                string frontArea = line.Substring(0, nextSymbolIndex);
                float number;
                if (frontArea != "")
                {
                    if (float.TryParse(frontArea, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out number))
                    {
                        equation.Add(new EEconstant(number));
                    }
                    else
                    {
                        equation.Add(new EEvariable(frontArea));
                    }
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
                    case '(':
                    case ')':
                        equation.Add(new EEbracket(currentSymbol));
                        break;
                }
                line = line.Substring(nextSymbolIndex + 1);
            }
        }

        public float CalculateValue(CetchUpObject cetchUpObject)
        {
            int i = -1;
            return CalculateValue(cetchUpObject, ref i);
        }

        public float CalculateValue(CetchUpObject cetchUpObject, ref int i)
        {
            i++;
            float value = GetValueFromValueElement(cetchUpObject, equation[i]);
            EEmodifier lastMod = new EEmodifier(EEmodifier.ModifierType.Add);
            while ((i++) < equation.Count - 1)
            {
                if (equation[i] is EEmodifier)
                {
                    lastMod = (EEmodifier)equation[i];
                    continue;
                }
                float calcValue = 0;
                if (equation[i] is EEbracket)
                {
                    if (((EEbracket)equation[i]).isStart)
                    {
                        calcValue = CalculateValue(cetchUpObject, ref i);
                    }
                    else
                    {
                        return value;
                    }
                }
                if (equation[i] is EEconstant || equation[i] is EEvariable)
                {
                    calcValue = GetValueFromValueElement(cetchUpObject, equation[i]);
                }
                switch (lastMod.modtype)
                {
                    case EEmodifier.ModifierType.Add: value += calcValue; break;
                    case EEmodifier.ModifierType.Subtract: value -= calcValue; break;
                    case EEmodifier.ModifierType.Multiply: value *= calcValue; break;
                    case EEmodifier.ModifierType.Divide: value /= calcValue; break;
                }
            }
            return value;
        }

        private float GetValueFromValueElement(CetchUpObject cetchUpObject, IEquationElement element)
        {
            if (element is EEconstant) { return ((EEconstant)element).constantValue; }
            if (element is EEvariable) { return cetchUpObject.GetValue(((EEvariable)element).variableName); }
            throw new ArgumentException("Expected a value");
        }

        private List<CetchValue> GetDependentValues(CetchUpObject cetchUpObject)
        {
            List<CetchValue> result = new List<CetchValue>();
            foreach (IEquationElement element in equation)
            {
                if (element is EEvariable)
                {
                    EEvariable depVar = (EEvariable)element;
                    result.Add(cetchUpObject.GetCetchValue(depVar.variableName));
                }
            }
            return result;
        }

        public void OnVariableChanged(object sender, CetchValue.ChangedEventArgs args)
        {
            args.cetchUpObject.GetCetchValue(modifiedValue).ModifyValue(this);
        }
    }
}