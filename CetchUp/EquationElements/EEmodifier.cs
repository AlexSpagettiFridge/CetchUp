namespace CetchUp.EquationElements
{
    internal struct EEmodifier
    {
        public readonly ModifierType modtype;

        public EEmodifier(ModifierType modtype)
        {
            this.modtype = modtype;
        }

        public EEmodifier(char symbol){
            switch(symbol){
                case '+': modtype = ModifierType.Add;
                case '-': modtype = ModifierType.Subtract;
                case '*': modtype = ModifierType.Multiply;
                case '/': modtype = ModifierType.Divide;
            }
        }

        public enum ModifierType{
            Multiply,Divide,Add,Subtract
        }
    }
}