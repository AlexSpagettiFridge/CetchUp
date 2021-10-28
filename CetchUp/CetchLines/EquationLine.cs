using System;
using System.Collections.Generic;
using System.Globalization;
using CetchUp.EquationElements;

namespace CetchUp.CetchLines
{
    internal class EquationLine : ICetchLine
    {
        private bool isMultiplier = false;
        private List<IEquationElement> equation = new List<IEquationElement>();
        private bool isValueLocal = false;
        private string modifiedValue;

        public bool IsMultiplier => isMultiplier;
        public event EventHandler<EquationLine> removed;

        public EquationLine(string cetchLine, CetchModifier cetchModifier)
        {
            PopulateFromCetchLine(cetchLine, cetchModifier);
        }

        public void JoinObject(CetchModifierEntry cetchModifierEntry)
        {
            CetchValueCollection valueParent = cetchModifierEntry.CetchUpObject;
            if (isValueLocal) { valueParent = cetchModifierEntry; }

            CetchValue cetchValue = valueParent.GetCetchValue(modifiedValue);
            cetchValue.AddModedValue(this, cetchModifierEntry);
            cetchValue.ModifyValue(this);
            foreach (CetchValue depVariable in GetDependentValues(cetchModifierEntry))
            {
                depVariable.changed += OnVariableChanged;
            }
        }

        public void Remove(CetchModifierEntry cetchModifierEntry)
        {
            foreach (CetchValue depVariable in GetDependentValues(cetchModifierEntry))
            {
                depVariable.changed -= OnVariableChanged;
            }
            if (removed != null) { removed.Invoke(this, this); }
        }

        private void PopulateFromCetchLine(string line, CetchModifier cetchModifier, bool preShortened = false)
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

                if (frontArea != "")
                {
                    equation.Add(EquationHelper.ParseValueElement(frontArea, cetchModifier));
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

        public float CalculateValue(CetchModifierEntry cetchModifierEntry)
        {
            int i = -1;
            return CalculateValue(cetchModifierEntry, ref i);
        }

        public float CalculateValue(CetchModifierEntry cetchModifierEntry, ref int i)
        {
            i++;
            float value = EquationHelper.GetValueFromValueElement(cetchModifierEntry, equation[i]);
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
                        calcValue = CalculateValue(cetchModifierEntry, ref i);
                    }
                    else
                    {
                        return value;
                    }
                }
                if (equation[i] is EEconstant || equation[i] is EEvariable || equation[i] is EElocalVariable)
                {
                    calcValue = EquationHelper.GetValueFromValueElement(cetchModifierEntry, equation[i]);
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

        private List<CetchValue> GetDependentValues(CetchModifierEntry cetchModifierEntry)
        {
            List<CetchValue> result = new List<CetchValue>();
            foreach (IEquationElement element in equation)
            {
                if (element is EEvariable)
                {
                    EEvariable depVar = (EEvariable)element;
                    result.Add(cetchModifierEntry.CetchUpObject.GetCetchValue(depVar.variableName));
                }
                if (element is EElocalVariable)
                {
                    EElocalVariable depLocVar = (EElocalVariable)element;
                    result.Add(cetchModifierEntry.GetCetchValue(depLocVar.variableName));
                }
            }
            return result;
        }

        public void OnVariableChanged(object sender, CetchValue.ChangedEventArgs args)
        {
            CetchValueCollection valueCollection = args.cetchModifierEntry;
            if (!isValueLocal){
                valueCollection = args.cetchModifierEntry.CetchUpObject;
            }
            valueCollection.GetCetchValue(modifiedValue).ModifyValue(this);
        }
    }
}