using System.Collections.Generic;

namespace CetchUp
{
    /// <summary>
    /// Every <see cref="CetchUp.CetchUpObject"/> has a <see cref="CetchUp.CetchModifierEntry"/> for every <see cref="CetchUp.CetchModifier"/>
    /// applied to it. The <see cref="CetchUp.CetchModifierEntry"/> also contains a collection of <see cref="CetchUp.CetchValue"/>s which
    /// represent local variables.
    /// </summary>
    public class CetchModifierEntry : CetchValueCollection
    {
        private CetchUpObject cetchUpObject;
        private CetchModifier cetchModifier;

        /// <summary>
        /// A reference to the <see cref="CetchUp.CetchUpObject" it is part of./>
        /// </summary>
        public CetchUpObject CetchUpObject => cetchUpObject;

        /// <summary>
        /// The <see cref="CetchUp.CetchModifier"/> the <see cref="CetchUp.CetchModifierEntry"/> refers to.
        /// </summary>
        public CetchModifier CetchModifier => cetchModifier;

        public CetchModifierEntry(CetchUpObject cetchUpObject, CetchModifier cetchModifier)
        {
            this.cetchUpObject = cetchUpObject;
            this.cetchModifier = cetchModifier;
        }

        public List<CetchValue> GetEffectedValues()
        {
            List<CetchValue> result = new List<CetchValue>();
            foreach (ICetchLine line in CetchModifier.Lines)
            {
                line.GetEffectedValues(this, ref result);
            }
            return result;
        }

        internal void GetEffectedValues(ref List<CetchValue> effectedValues)
        {
            foreach (ICetchLine line in CetchModifier.Lines)
            {
                line.GetEffectedValues(this, ref effectedValues);
            }
        }
    }
}