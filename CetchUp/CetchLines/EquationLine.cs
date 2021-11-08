using System;
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
        public event EventHandler<EquationLine> removed;

        public EquationLine(string cetchLine, CetchModifier cetchModifier)
        {
            cetchLine = Regex.Replace(cetchLine, @"[\s]", "");
            modifiedValue = Regex.Match(cetchLine, "^.*(?==)").Value;
            dependencies = new List<string>();
            equation = new EEequation(Regex.Match(cetchLine, @"=\K.*(?:;$)").Value, ref dependencies);
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

        private List<CetchValue> GetDependentValues(CetchModifierEntry cetchModifierEntry)
        {
            List<CetchValue> result = new List<CetchValue>();
            foreach(string dep in dependencies)
            {
                if (dep.StartsWith("#")){
                    cetchModifierEntry.GetValue(dep);
                    continue;
                }
                cetchModifierEntry.CetchUpObject.GetValue(dep);
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
    }
}