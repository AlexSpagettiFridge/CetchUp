using System;
using System.Collections.Generic;
using CetchUp.EquationElements;

namespace CetchUp
{
    public class CetchUpObject : CetchValueCollection
    {
        private List<CetchModifierEntry> modifierEntries = new List<CetchModifierEntry>();
        private event EventHandler<LocalValueChangedEventArgs> localValueChanged;

        public CetchModifier MakeModifer()
        {
            string genString = "";
            foreach (KeyValuePair<string, CetchValue> item in values)
            {
                genString += $"{item.Key}={item.Value.Total / item.Value.Multiplier};";
                if (item.Value.Multiplier != 1)
                {
                    genString += $"{item.Key}%{item.Value.Multiplier};";
                }
            }
            return new CetchModifier(genString);
        }

        /// <summary>
        /// Applies a CetchModifier to this CetchUpObject.
        /// This way the CetchValues inside this CetchUpObject will be modified.
        /// </summary>
        /// <param name="modifier">The modifier that should be applied.</param>
        public CetchModifierEntry ApplyModifier(CetchModifier modifier, CetchUpObject[] references = null)
        {
            CetchModifierEntry entry = new CetchModifierEntry(this, modifier,references);
            modifierEntries.Add(entry);
            modifier.ModifyCetchObject(entry);
            entry.valueChanged += OnLocalValueChanged;
            return entry;
        }

        public float CallLine(string line)
        {
            IEquationElement ee = EquationHelper.CreateEquationElementFromLine(line);
            return ee.GetValue();
        }

        public bool TryRemoveModifier(CetchModifier modifier)
        {
            foreach (CetchModifierEntry entry in modifierEntries)
            {
                if (entry.CetchModifier == modifier)
                {
                    modifier.RemoveFromCetchObject(entry);
                    modifierEntries.Remove(entry);
                    entry.valueChanged -= OnLocalValueChanged;
                    return true;
                }
            }
            return false;
        }

        public void RemoveModifierEntry(CetchModifierEntry entry)
        {
            if (!modifierEntries.Contains(entry)) { return; }

        }

        private void OnLocalValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (localValueChanged != null)
            {
                localValueChanged.Invoke(this, new LocalValueChangedEventArgs((CetchModifierEntry)sender,
                args.changedValue, args.newValue));
            }
        }

        public class LocalValueChangedEventArgs : EventArgs
        {
            public CetchModifierEntry cetchModifierEntry;
            public CetchValue cetchValue;
            public float newValue;

            public LocalValueChangedEventArgs(CetchModifierEntry cetchModifierEntry, CetchValue cetchValue, float newValue)
            {
                this.cetchModifierEntry = cetchModifierEntry;
                this.cetchValue = cetchValue;
                this.newValue = newValue;
            }
        }

        public List<CetchValue> GetEffectedValues()
        {
            List<CetchValue> result = new List<CetchValue>();
            foreach (CetchModifierEntry modifierEntry in modifierEntries)
            {
                modifierEntry.GetEffectedValues(ref result);
            }
            return result;
        }

        /// <summary>
        /// Reroll all rolls inside all <see cref="CetchUp.CetchModifier"> applied to this <see cref="CetchUp.CetchUpObject">
        /// </summary>
        public void Reroll()
        {
            foreach(CetchModifierEntry entry in modifierEntries)
            {
                entry.Reroll();
            }
        }
    }
}