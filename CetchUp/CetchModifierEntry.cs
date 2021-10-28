using System.Collections.Generic;

namespace CetchUp
{
    public class CetchModifierEntry : CetchValueCollection
    {
        private CetchUpObject cetchUpObject;
        private CetchModifier cetchModifier;

        public CetchModifierEntry(CetchUpObject cetchUpObject, CetchModifier cetchModifier)
        {
            this.cetchUpObject = cetchUpObject;
            this.cetchModifier = cetchModifier;
        }

        public CetchUpObject CetchUpObject => cetchUpObject;

        public CetchModifier CetchModifier => cetchModifier;
    }
}