using System.Collections.Generic;
using System;

namespace CetchUp
{
    public class CetchValue
    {
        private CetchUpObject cetchUpObject;
        private float baseValue;
        private float value;
        private float multiplier;
        private List<EquationLine> valueMods = new List<EquationLine>();

        public float Total => (baseValue + value) * multiplier;
        public event EventHandler<float> changed;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                if (changed != null) { changed.Invoke(this, Total); }
            }
        }

        public CetchValue(CetchUpObject cetchUpObject, float value, float multiplier = 1)
        {
            this.cetchUpObject = cetchUpObject;
            baseValue = value;
            this.value = value;
            this.multiplier = multiplier;
        }

        internal void ModifyValue(EquationLine modV)
        {
            if (modV.IsMultiplier)
            {
                multiplier -= modV.Value;
                multiplier += modV.CalculateValue(cetchUpObject);
            }
            else
            {
                value -= modV.Value;
                value += modV.CalculateValue(cetchUpObject);
            }
            modV.removed += OnModedValueRemoved;
            if (changed != null) { changed.Invoke(this, Total); }
        }

        internal void AddModedValue(EquationLine modedValue)
        {
            valueMods.Add(modedValue);
            modedValue.removed += OnModedValueRemoved;
        }

        private void OnModedValueRemoved(object sender, EquationLine modedValue)
        {
            valueMods.Remove(modedValue);
            if (modedValue.IsMultiplier)
            {
                multiplier -= modedValue.Value;
            }
            else
            {
                value -= modedValue.Value;
            }
            modedValue.removed -= OnModedValueRemoved;
        }

    }
}