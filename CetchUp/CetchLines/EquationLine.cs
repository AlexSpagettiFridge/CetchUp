using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CetchUp.EquationElements;

namespace CetchUp.CetchLines
{
    internal class EquationLine : ICetchLine
    {
        private bool isMultiplier = false;
        private EEequation equation;
        private bool isValueLocal = false;
        private string modifiedValue;
        private List<string> dependencies;

        public bool IsMultiplier => isMultiplier;
        public string ModifiedValue => modifiedValue;
        public EEequation Equation => equation;

        public event EventHandler<EquationLine> removed;

        public EquationLine(string cetchLine)
        {
            GroupCollection groups = Regex.Match(cetchLine, "(.*)([=%])(.*)").Groups;

            modifiedValue = Regex.Replace(groups[1].Value,@"\s","");
            isMultiplier = groups[2].Value == "%";
            dependencies = new List<string>();
            equation = new EEequation(groups[3].Value, ref dependencies);
        }

        public EquationLine(string modifiedValue, EEequation equation, List<string> dependencies, bool isMultiplier)
        {
            this.modifiedValue = modifiedValue;
            this.equation = equation;
            this.dependencies = dependencies;
            this.isMultiplier = isMultiplier;
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
            return new EquationLine(modifiedValue, (EEequation)equation.Clone(), newDependencies, isMultiplier);
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
            CetchValueCollection valueCollection = args.cetchModifierEntry;
            if (!isValueLocal)
            {
                valueCollection = args.cetchModifierEntry.CetchUpObject;
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
            equation.InsertVariable(varName,value);
        }
    }
}