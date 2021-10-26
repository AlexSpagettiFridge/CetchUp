using System;
using System.Collections.Generic;

namespace CetchUp
{
    public class CetchUpObject
    {
        private Dictionary<string, CetchValue> values = new Dictionary<string, CetchValue>();
        private List<CetchModifier> modifiers = new List<CetchModifier>();
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
            modifiers.Add(modifier);
            modifier.ModifyCetchObject(this);
        }

        public void RemoveModifier(CetchModifier modifier)
        {
            modifier.RemoveFromCetchObject(this);
            modifiers.Remove(modifier);
        }

        public void AddNewValue(string name, float defaultValue = 0, float defaultModifier = 1)
        {
            CetchValue value = new CetchValue(this, defaultValue, defaultModifier);
            values.Add(name, value);
            value.changed += OnValueChanged;
        }

        public float GetValue(string valueName)
        {
            try
            {
                return values[valueName].Total;
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception($"The CetchUpObject does not contain {e.Data["Key"]}");
            }
        }

        public CetchValue GetCetchValue(string valueName)
        {
            if (!values.ContainsKey(valueName))
            {
                return null;
            }
            return values[valueName];
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