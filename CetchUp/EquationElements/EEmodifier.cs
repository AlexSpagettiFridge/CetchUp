namespace CetchUp.EquationElements
{
    internal struct EEmodifier : IEquationElement
    {
        public readonly ModifierType modtype;

        public EEmodifier(ModifierType modtype)
        {
            this.modtype = modtype;
        }

        public EEmodifier(char symbol){
            switch(symbol){
                case '-': modtype = ModifierType.Subtract; break;
                case '*': modtype = ModifierType.Multiply; break;
                case '/': modtype = ModifierType.Divide; break;
                default : modtype = ModifierType.Add; break;
            }
        }

        public enum ModifierType{
            Multiply,Divide,Add,Subtract
        }
    }
}