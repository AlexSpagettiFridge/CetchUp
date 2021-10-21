namespace CetchUp.EquationElements
{
    internal struct EEvariable : IEquationElement
    {
        public readonly string variableName;

        public EEvariable(string variableName)
        {
            this.variableName = variableName;
        }
    }
}