namespace CetchUp
{
    internal interface ICetchLine
    {
        void JoinObject(CetchUpObject cetchUpObject);
        void Remove(CetchUpObject cetchUpObject);
    }
}