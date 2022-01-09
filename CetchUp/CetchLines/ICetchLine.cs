using System;
using System.Collections.Generic;

namespace CetchUp
{
    internal interface ICetchLine : ICloneable
    {
        void JoinObject(CetchModifierEntry cetchModifierEntry);
        void Remove(CetchModifierEntry cetchModifierEntry);
        void ModifyByValue(float value);
        void InsertVariable(string varName, float value);
        void GetEffectedValues(CetchModifierEntry cetchModifierEntry, ref List<CetchValue> valueList);
    }
}