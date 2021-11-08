namespace CetchUp.EquationElements
{
    internal class EEsymbol : IEquationElement
    {
        public char symbol;

        public EEsymbol(string line)
        {
            symbol = line.ToCharArray()[0];
        }

        public float GetValue(CetchModifierEntry cetchModifierEntry)
        {
            throw new System.NotImplementedException();
        }
    }
}