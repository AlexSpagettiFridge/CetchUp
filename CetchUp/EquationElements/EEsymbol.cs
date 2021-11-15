namespace CetchUp.EquationElements
{
    internal struct EEsymbol : IEquationElement
    {
        public char symbol;

        public EEsymbol(string line)
        {
            symbol = line.ToCharArray()[0];
        }

        public EEsymbol(char symbol)
        {
            this.symbol = symbol;
        }

        public bool IsFactorBasedOperation => (symbol == '*' || symbol == '/');

        public float GetValue()
        {
            throw new System.NotImplementedException();
        }

        public float GetValue(CetchModifierEntry cetchModifierEntry)
        {
            throw new System.NotImplementedException();
        }

        public IEquationElement Copy() => new EEsymbol(symbol); 

        public override string ToString()
        {
            return symbol.ToString();
        }
    }
}