using System.Collections.Generic;
using System;

namespace CetchUp
{
    internal class CetchValue
    {
        private float baseValue;
        private float value;
        private float multiplier;
        private List<ModedValue> valueMods = List<ModedValue>();

        public float Total => value * multiplier;
        public event EventHandler<float> changed;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                valueChanged.Invoke(Total);
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
            changed.Invoke(Total);
        }

        public AddModedValue(ModedValue modedValue)
        {
            valueMods.Add(modedValue);
            modedValue.removed += OnModedValueRemoved;
        }

        private OnModedValueRemoved(object sender, ModedValue modedValue)
        {
            valueMods.Remove(modedValue);
        }

    }
}