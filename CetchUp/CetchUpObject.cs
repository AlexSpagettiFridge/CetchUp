using System;
using System.Collections.Generic;

namespace CetchUp
{
    public class CetchUpObject
    {
        private Dictionary<string, float> values = new Dictionary<string, float>();
        private List<CetchModifier> modifiers = List<CetchModifier>();

        public float GetValue(string valueName)
        {
            try
            {
                return values[valueName];
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception($"The CetchUpObject does not contain {e.Data["Key"]}");
            }
        }
    }
}