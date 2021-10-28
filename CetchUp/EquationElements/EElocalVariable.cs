namespace CetchUp.EquationElements
{
    public class EElocalVariable : IEquationElement
    {
        public readonly string variableName;
        public readonly CetchModifier cetchModifier;

        public EElocalVariable(string variableName, CetchModifier cetchModifier)
        {
            this.variableName = variableName;
            this.cetchModifier = cetchModifier;
        }
    }
}