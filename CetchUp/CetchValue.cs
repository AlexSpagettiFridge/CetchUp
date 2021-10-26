using System.Collections.Generic;
using System;
using CetchUp.CetchLines;

namespace CetchUp
{
    public class CetchValue
    {
        private CetchUpObject cetchUpObject;
        private float baseValue;
        private float value = 0;
        private float multiplier;
        private Dictionary<EquationLine, float> valueMods = new Dictionary<EquationLine, float>();

        public float Total => (baseValue + value) * multiplier;
        public float Multiplier => multiplier;
        public event EventHandler<ChangedEventArgs> changed;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                if (changed != null) { changed.Invoke(this, new ChangedEventArgs(cetchUpObject, Total)); }
            }
        }

        public CetchValue(CetchUpObject cetchUpObject, float value, float multiplier = 1)
        {
            this.cetchUpObject = cetchUpObject;
            baseValue = value;
            this.multiplier = multiplier;
        }

        internal void ModifyValue(EquationLine modV)
        {
            float modVValue = modV.CalculateValue(cetchUpObject);
            if (modV.IsMultiplier)
            {
                multiplier -= valueMods[modV];
                multiplier += modVValue;
            }
            else
            {
                value -= valueMods[modV];
                value += modVValue;
            }
            valueMods[modV] = modVValue;
            if (changed != null) { changed.Invoke(this, new ChangedEventArgs(cetchUpObject, Total)); }
        }

        internal void AddModedValue(EquationLine modedValue)
        {
            valueMods.Add(modedValue, 0);
            modedValue.removed += OnModedValueRemoved;
        }

        private void OnModedValueRemoved(object sender, EquationLine modedValue)
        {
            if (modedValue.IsMultiplier)
            {
                multiplier -= valueMods[modedValue];
            }
            else
            {
                value -= valueMods[modedValue];
            }
            modedValue.removed -= OnModedValueRemoved;
            valueMods.Remove(modedValue);
            if (changed != null) { changed.Invoke(this, new ChangedEventArgs(cetchUpObject, Total)); }
        }

        public class ChangedEventArgs : EventArgs
        {
            public float newValue;
            public CetchUpObject cetchUpObject;

            public ChangedEventArgs(CetchUpObject cetchUpObject, float newValue)
            {
                this.cetchUpObject = cetchUpObject;
                this.newValue = newValue;
            }
        }

    }
}