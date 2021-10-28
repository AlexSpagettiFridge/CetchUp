using System;
using System.Collections.Generic;

namespace CetchUp
{
    public class CetchUpObject : CetchValueCollection
    {
        private List<CetchModifierEntry> modifierEntries = new List<CetchModifierEntry>();
        public event EventHandler<ValueChangedEventArgs> valueChanged;

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

        public void ApplyModifier(CetchModifier modifier)
        {
            CetchModifierEntry entry = new CetchModifierEntry(this, modifier);
            modifierEntries.Add(entry);
            modifier.ModifyCetchObject(entry);
        }

        public bool TryRemoveModifier(CetchModifier modifier)
        {
            foreach (CetchModifierEntry entry in modifierEntries)
            {
                if (entry.CetchModifier == modifier)
                {
                    modifier.RemoveFromCetchObject(entry);
                    modifierEntries.Remove(entry);
                    return true;
                }
            }
            return false;
        }

        private void OnValueChanged(object sender, CetchValue.ChangedEventArgs args)
        {
            if (valueChanged != null)
            {
                valueChanged.Invoke(this, new ValueChangedEventArgs((CetchValue)sender, args.newValue));
            }
        }

        public class ValueChangedEventArgs
        {
            public CetchValue changedValue;
            public float newValue;

            public ValueChangedEventArgs(CetchValue changedValue, float newValue)
            {
                this.changedValue = changedValue;
                this.newValue = newValue;
            }
        }
    }
}