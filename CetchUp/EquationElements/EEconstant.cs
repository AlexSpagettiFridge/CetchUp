namespace CetchUp.EquationElements
{
    internal struct EEconstant : IEquationElement
    {
        public readonly float constantValue;

        public EEconstant(float constantValue)
        {
            this.constantValue = constantValue;
        }
    }
}