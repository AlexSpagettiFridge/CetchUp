using System.Collections.Generic;
using System;
using CetchUp.CetchLines;

namespace CetchUp
{
    public class CetchValue
    {
        private readonly CetchUpObject cetchUpObject;
        private readonly string name;
        private float baseValue;
        private float value = 0;
        private float multiplier;
        private List<ValueModEntry> valueMods = new List<ValueModEntry>();

        public string Name => name;
        public float Total => (baseValue + value) * multiplier;
        public float Multiplier => multiplier;
        public event EventHandler<ChangedEventArgs> changed;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                if (changed != null) { changed.Invoke(this, new ChangedEventArgs(null, Total)); }
            }
        }

        public CetchValue(CetchUpObject cetchUpObject, string name, float value, float multiplier = 1)
        {
            this.cetchUpObject = cetchUpObject;
            this.name = name;
            baseValue = value;
            this.multiplier = multiplier;
        }

        private ValueModEntry GetValueModEntry(EquationLine equation)
        {
            foreach (ValueModEntry entry in valueMods)
            {
                if (entry.equation == equation)
                {
                    return entry;
                }
            }
            throw new KeyNotFoundException(equation.ToString());
        }

        internal void ModifyValue(EquationLine modV)
        {
            ValueModEntry entry = GetValueModEntry(modV);
            float modVValue = modV.Calculate(entry.origin);
            if (modV.IsMultiplier)
            {
                multiplier -= entry.value;
                multiplier += modVValue;
            }
            else
            {
                value -= entry.value;
                value += modVValue;
            }
            entry.value = modVValue;
            if (changed != null) { changed.Invoke(this, new ChangedEventArgs(entry.origin, Total)); }
        }

        internal void AddModedValue(EquationLine equation, CetchModifierEntry cetchModifierEntry)
        {
            valueMods.Add(new ValueModEntry(equation, cetchModifierEntry, 0));
            equation.removed += OnModedValueRemoved;
        }

        private void OnModedValueRemoved(object sender, EquationLine modedValue)
        {
            ValueModEntry entry = GetValueModEntry(modedValue);
            if (modedValue.IsMultiplier)
            {
                multiplier -= entry.value;
            }
            else
            {
                value -= entry.value;
            }
            modedValue.removed -= OnModedValueRemoved;
            valueMods.Remove(entry);
            if (changed != null) { changed.Invoke(this, new ChangedEventArgs(entry.origin, Total)); }
        }

        public class ChangedEventArgs : EventArgs
        {
            public float newValue;
            public CetchModifierEntry cetchModifierEntry;

            public ChangedEventArgs(CetchModifierEntry cetchModifierEntry, float newValue)
            {
                this.cetchModifierEntry = cetchModifierEntry;
                this.newValue = newValue;
            }
        }

        private class ValueModEntry
        {
            public EquationLine equation;
            public CetchModifierEntry origin;
            public float value;

            public ValueModEntry(EquationLine equation, CetchModifierEntry origin, float value)
            {
                this.equation = equation;
                this.origin = origin;
                this.value = value;
            }
        }
    }
}