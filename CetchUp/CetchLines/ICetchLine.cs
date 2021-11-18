using System;

namespace CetchUp
{
    internal interface ICetchLine : ICloneable
    {
        void JoinObject(CetchModifierEntry cetchModifierEntry);
        void Remove(CetchModifierEntry cetchModifierEntry);
        void ModifyByValue(float value);
    }
}