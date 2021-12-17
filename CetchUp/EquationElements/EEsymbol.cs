using System.Collections.Generic;

namespace CetchUp.EquationElements
{
    internal class EEsymbol : IEquationElement
    {
        public char symbol;

        public EEsymbol() { }

        public EEsymbol(char symbol)
        {
            this.symbol = symbol;
        }

        public void Init(string line)
        {
            symbol = line.ToCharArray()[0];
        }

        public void Init(string line, ref List<string> dependencies) => Init(line);

        public bool IsFactorBasedOperation => (symbol == '*' || symbol == '/');

        public float GetValue()
        {
            throw new System.NotImplementedException();
        }

        public float GetValue(CetchModifierEntry cetchModifierEntry)
        {
            throw new System.NotImplementedException();
        }

        public object Clone() => new EEsymbol(symbol); 

        public override string ToString()
        {
            return symbol.ToString();
        }
    }
}