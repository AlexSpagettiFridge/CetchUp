using System;
using System.Collections.Generic;

namespace CetchUp
{
    public abstract class CetchValueCollection
    {
        internal Dictionary<string, CetchValue> values = new Dictionary<string, CetchValue>();

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
                if (this is CetchUpObject)
                {
                    values.Add(valueName, new CetchValue((CetchUpObject)this, valueName, 0));
                }
                else
                {
                    values.Add(valueName, new CetchValue(((CetchModifierEntry)this).CetchUpObject, valueName, 0));
                }

            }
            return values[valueName];
        }
    }
}