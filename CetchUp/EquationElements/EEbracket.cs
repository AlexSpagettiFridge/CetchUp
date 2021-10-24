namespace CetchUp.EquationElements
{
    public class EEbracket : IEquationElement
    {
        public readonly bool isStart;

        public EEbracket(bool isStart)
        {
            this.isStart = isStart;
        }

        public EEbracket(char symbol)
        {
            isStart = (symbol == '(');
        }
    }
}