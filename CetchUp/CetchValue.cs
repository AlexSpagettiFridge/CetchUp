using System.Collections.Generic;
using System;
using System.Linq;
using CetchUp.CetchLines;

namespace CetchUp
{
    public class CetchValue
    {
        private readonly CetchUpObject cetchUpObject;
        private readonly string name;
        private float baseValue;
        private float value = 0, multiplier = 1, min = 0, max = 0, minMod = 1, maxMod = 1;
        private List<ValueModEntry> valueMods = new List<ValueModEntry>();

        public string Name => name;
        public float Total
        {
            get => ClampValue((baseValue + value) * multiplier);
            set
            {
                ClampValue(value);
                BaseValue = (value - this.value) / multiplier;
            }
        }
        public float? Min
        {
            get
            {
                if (CountValuePartMods(ValuePart.Min) == 0) { return null; }
                return min * minMod;
            }
        }

        public float? Max
        {
            get
            {
                if (CountValuePartMods(ValuePart.Max) == 0) { return null; }
                return max * maxMod;
            }
        }

        public float Multiplier => multiplier;

        /// <summary>
        /// Invokes this whenever this objects value has changes
        /// </summary>
        public event EventHandler<ChangedEventArgs> Changed;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                Changed?.Invoke(this, new ChangedEventArgs(cetchUpObject, Total));
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

        private void ChangeValuePart(ValuePart part, float mod)
        {
            switch (part)
            {
                case ValuePart.Value: value += mod; break;
                case ValuePart.Modifier: multiplier += mod; break;
                case ValuePart.Min: min += mod; break;
                case ValuePart.Max: max += mod; break;
                case ValuePart.MinMod: minMod += mod; break;
                case ValuePart.MaxMod: maxMod += mod; break;
            }
        }

        private int CountValuePartMods(ValuePart part)
        {
            return valueMods.Count<ValueModEntry>((ValueModEntry x) => x.valuePart == part);
        }

        private float ClampValue(float value)
        {
            if (CountValuePartMods(ValuePart.Min) != 0)
            {
                value = (float)Math.Max((float)Min, value);
            }
            if (CountValuePartMods(ValuePart.Max) != 0)
            {
                value = (float)Math.Min((float)Max, value);
            }
            return value;
        }
        internal void ModifyValue(EquationLine modV)
        {
            ValueModEntry entry = GetValueModEntry(modV);
            float modVValue = modV.Calculate(entry.origin);
            ChangeValuePart(modV.ValuePart, -entry.value);
            ChangeValuePart(modV.ValuePart, modVValue);
            entry.value = modVValue;
            Changed?.Invoke(this, new ChangedEventArgs(entry.origin, Total));
        }

        internal void AddModedValue(EquationLine equation, CetchModifierEntry cetchModifierEntry)
        {
            valueMods.Add(new ValueModEntry(equation, cetchModifierEntry, 0, equation.ValuePart));
            equation.removed += OnModedValueRemoved;
        }

        private void OnModedValueRemoved(object sender, EquationLine modedValue)
        {
            ValueModEntry entry = GetValueModEntry(modedValue);

            ChangeValuePart(modedValue.ValuePart, -entry.value);

            modedValue.removed -= OnModedValueRemoved;
            valueMods.Remove(entry);
            if (Changed != null) { Changed.Invoke(this, new ChangedEventArgs(entry.origin, Total)); }
        }

        public class ChangedEventArgs : EventArgs
        {
            public float newValue;
            public CetchValueCollection cetchModifierCollection;

            public ChangedEventArgs(CetchValueCollection cetchModifierEntry, float newValue)
            {
                this.cetchModifierCollection = cetchModifierEntry;
                this.newValue = newValue;
            }
        }

        public List<ValueModEntry> GetAllValueModEntries() => valueMods;
        public IEnumerable<ValueModEntry> GetValueModEntriesWhere(Func<ValueModEntry, bool> predicate) => valueMods.Where<ValueModEntry>(predicate);
        public IEnumerable<ValueModEntry> GetAllValueModValueModifiers() => GetValueModEntriesWhere((e) => e.equation.ValuePart == ValuePart.Value);
        public IEnumerable<ValueModEntry> GetAllValueModModifiers() => GetValueModEntriesWhere((e) => e.equation.ValuePart == ValuePart.Modifier);


        internal enum ValuePart
        {
            Value, Modifier, Min, Max, MinMod, MaxMod
        }
    }
}