using System.Collections.Generic;
using System;

namespace CetchUp
{
    internal class CetchValue
    {
        private float baseValue;
        private float value;
        private float multiplier;
        private List<ModedValue> valueMods = new List<ModedValue>();

        public float Total => value * multiplier;
        public event EventHandler<float> changed;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                changed.Invoke(this, Total);
            }
        }

        public CetchValue(float value, float multiplier = 1)
        {
            baseValue = value;
            this.value = value;
            this.multiplier = multiplier;
        }

        public void ModifyValue(ModedValue modV)
        {
            if (modV.IsMultiplier)
            {
                multiplier -= modV.Value;
                multiplier += modV.CalculateValue();
            }
            else
            {
                value -= modV.Value;
                value += modV.CalculateValue();
            }
            changed.Invoke(this, Total);
        }

        public void AddModedValue(ModedValue modedValue)
        {
            valueMods.Add(modedValue);
            modedValue.removed += OnModedValueRemoved;
        }

        private void OnModedValueRemoved(object sender, ModedValue modedValue)
        {
            valueMods.Remove(modedValue);
        }

    }
}