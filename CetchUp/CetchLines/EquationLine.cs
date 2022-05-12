using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CetchUp.EquationElements;

namespace CetchUp.CetchLines
{
    internal class EquationLine : ICetchLine
    {
        private CetchValue.ValuePart valuePart;
        private EEequation equation;
        private bool isValueLocal = false;
        private string modifiedValue;
        private List<string> dependencies;

        public CetchValue.ValuePart ValuePart => valuePart;
        public string ModifiedValue => modifiedValue;
        public EEequation Equation => equation;

        public event EventHandler<EquationLine> removed;

        public EquationLine(string cetchLine)
        {
            GroupCollection groups = Regex.Match(cetchLine, "(.*)([=%mM])(.*)").Groups;

            modifiedValue = Regex.Replace(groups[1].Value, @"\s", "");
            switch (groups[2].Value)
            {
                case "=": valuePart = CetchValue.ValuePart.Value; break;
                case "%": valuePart = CetchValue.ValuePart.Modifier; break;
                case "m": valuePart = CetchValue.ValuePart.Min; break;
                case "M": valuePart = CetchValue.ValuePart.Max; break;
            }
            dependencies = new List<string>();
            equation = new EEequation();
            equation.Init(groups[3].Value, ref dependencies);
        }

        public EquationLine(string modifiedValue, EEequation equation, List<string> dependencies, CetchValue.ValuePart valuePart)
        {
            this.modifiedValue = modifiedValue;
            this.equation = equation;
            this.dependencies = dependencies;
            this.valuePart = valuePart;
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
                depVariable.Changed += OnVariableChanged;
            }
        }

        public void Remove(CetchModifierEntry cetchModifierEntry)
        {
            foreach (CetchValue depVariable in GetDependentValues(cetchModifierEntry))
            {
                depVariable.Changed -= OnVariableChanged;
            }
            if (removed != null) { removed.Invoke(this, this); }
        }

        public float Calculate(CetchModifierEntry cetchModifierEntry)
        {
            return equation.GetValue(cetchModifierEntry);
        }

        public void ModifyByValue(float modifier)
        {
            IEquationElement moddedElement = equation * modifier;
            if (moddedElement is EEequation)
            {
                equation = (EEequation)moddedElement;
            }
            else
            {
                equation = new EEequation(new ArrayList { moddedElement });
            }
        }

        public object Clone()
        {
            List<string> newDependencies = new List<string>();
            foreach (string dep in dependencies) { newDependencies.Add((string)dep.Clone()); }
            return new EquationLine(modifiedValue, (EEequation)equation.Clone(), newDependencies, valuePart);
        }

        private List<CetchValue> GetDependentValues(CetchModifierEntry cetchModifierEntry)
        {
            List<CetchValue> result = new List<CetchValue>();
            foreach (string dep in dependencies)
            {
                if (dep.StartsWith("#"))
                {
                    result.Add(cetchModifierEntry.GetCetchValue(dep));
                    continue;
                }
                result.Add(cetchModifierEntry.CetchUpObject.GetCetchValue(dep));
            }
            return result;
        }

        public void OnVariableChanged(object sender, CetchValue.ChangedEventArgs args)
        {
            CetchValueCollection valueCollection = args.cetchModifierCollection;
            if (!isValueLocal && args.cetchModifierCollection is CetchModifierEntry cetchModifierEntry)
            {
                valueCollection = cetchModifierEntry.CetchUpObject;
            }
            valueCollection.GetCetchValue(modifiedValue).ModifyValue(this);
        }

        public override string ToString()
        {
            return $"{modifiedValue}: {equation.ToString()}";
        }

        public void TryShorten()
        {
            equation.TryShorten();
        }

        public void InsertVariable(string varName, float value)
        {
            equation.InsertVariable(varName, value);
        }

        public void GetEffectedValues(CetchModifierEntry cetchModifierEntry, ref List<CetchValue> valueList)
        {
            CetchValue cetchValue = cetchModifierEntry.GetCetchValue(modifiedValue);
            if (valueList.Contains(cetchValue)) { valueList.Add(cetchValue); }
        }

        public void Reroll(CetchModifierEntry cetchModifierEntry)
        {
            equation.Reroll();
            CetchValueCollection valueCollection = cetchModifierEntry;
            if (!isValueLocal)
            {
                valueCollection = cetchModifierEntry.CetchUpObject;
            }
            valueCollection.GetCetchValue(modifiedValue).ModifyValue(this);
        }
    }
}