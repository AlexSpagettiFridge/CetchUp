using System;
using System.Collections.Generic;

namespace CetchUp
{
    public class CetchUpObject
    {
        private Dictionary<string, CetchValue> values = new Dictionary<string, CetchValue>();
        private List<CetchModifier> modifiers = new List<CetchModifier>();

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
            values.Add(name, new CetchValue(this, defaultValue, defaultModifier));
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
    }
}