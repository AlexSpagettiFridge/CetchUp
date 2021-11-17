namespace CetchUp
{
    internal interface ICetchLine
    {
        void JoinObject(CetchModifierEntry cetchModifierEntry);
        void Remove(CetchModifierEntry cetchModifierEntry);
        void ModifyByValue(float value);
    }
}