using System;
using System.Collections.Generic;

namespace CetchUp
{
    public class CetchUpObject
    {
        private Dictionary<string, CetchValue> values = new Dictionary<string, CetchValue>();
        private List<CetchModifier> modifiers = List<CetchModifier>();

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

        protected internal CetchValue GetCetchValue(string valueName)
        {
            return values[valueName];
        }
    }
}