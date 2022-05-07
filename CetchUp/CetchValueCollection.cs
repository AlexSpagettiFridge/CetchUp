using System;
using System.Collections.Generic;

namespace CetchUp
{
    /// <summary>
    /// A collection of CetchValues.
    /// </summray>
    public abstract class CetchValueCollection
    {
        internal Dictionary<string, CetchValue> values = new Dictionary<string, CetchValue>();
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        public virtual float GetValue(string valueName, bool initWhenUnavailable = true)
        {
            return GetCetchValue(valueName, initWhenUnavailable).Total;
        }

        public virtual CetchValue GetCetchValue(string valueName, bool initWhenUnavailable = true)
        {
            if (!values.ContainsKey(valueName))
            {
                if (!initWhenUnavailable)
                {
                    throw new KeyNotFoundException(valueName);
                }
                CetchValue newCetchValue;
                if (this is CetchUpObject)
                {
                    newCetchValue = new CetchValue((CetchUpObject)this, valueName, 0);
                }
                else
                {
                    newCetchValue = new CetchValue(((CetchModifierEntry)this).CetchUpObject, valueName, 0);
                }

                values.Add(valueName, newCetchValue);
                newCetchValue.Changed += OnValueChanged;
            }
            return values[valueName];
        }

        private void OnValueChanged(object sender, CetchValue.ChangedEventArgs args)
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, new ValueChangedEventArgs((CetchValue)sender, args.newValue));
            }
        }

        public class ValueChangedEventArgs : EventArgs
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