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

        /// <summary>
        /// Whenever a CetchValue inside this collection has changed this Event will be called.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Returns the current Value of the CetchValue Object with the corresponding name
        /// </summary>
        /// <param name="valueName">The name the CetchValue is identified by.</param>
        /// <param name="initWhenUnavailable">Should the value be automatically initiated when none is found?</param>
        /// <returns>The current Value as a float</returns>
        public virtual float GetValue(string valueName, bool initWhenUnavailable = true)
        {
            return GetCetchValue(valueName, initWhenUnavailable).Total;
        }

        /// <summary>
        /// Returns the CetchValue Object with the given <paramref name="valueName"/>
        /// </summary>
        /// <param name="valueName">The name the CetchValue Object is identified with</param>
        /// <param name="initWhenUnavailable">When true creates a new CetchValue when none is found.</param>
        /// <returns></returns>
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

        /// <summary>
        /// <see cref="System.EventArgs"/> for <see cref="CetchUp.CetchValueCollection.ValueChanged"/>
        /// </summary>
        public class ValueChangedEventArgs : EventArgs
        {
            /// <summary>
            /// the CetchValue object that has been changed.
            /// </summary>
            public CetchValue ChangedValue;

            /// <summary>
            /// The New Value as a float
            /// </summary>
            public float NewValue;

            public ValueChangedEventArgs(CetchValue changedValue, float newValue)
            {
                this.ChangedValue = changedValue;
                this.NewValue = newValue;
            }
        }
    }
}