using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CetchUp
{
    public class CetchUpObject
    {
        private Dictionary<string, CetchValue> values = new Dictionary<string, CetchValue>();
        private List<CetchModifier> modifiers = new List<CetchModifier>();

        public void ApplyModifier(CetchModifier modifier)
        {
            modifiers.Add(modifier);
            modifier.ModifyCetchObject(this);
        }

        public void RemoveModifier(CetchModifier modifier){
            modifiers.Remove(modifier);
            modifier.RemoveFromCetchObject(this);
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