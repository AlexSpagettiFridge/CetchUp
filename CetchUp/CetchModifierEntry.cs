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
        private CetchUpObject[] referenceObjects;

        /// <summary>
        /// A reference to the <see cref="CetchUp.CetchUpObject" it is part of./>
        /// </summary>
        public CetchUpObject CetchUpObject => cetchUpObject;

        /// <summary>
        /// The <see cref="CetchUp.CetchModifier"/> the <see cref="CetchUp.CetchModifierEntry"/> refers to.
        /// </summary>
        public CetchModifier CetchModifier => cetchModifier;

        public CetchUpObject[] ReferenceObjects => referenceObjects;

        public CetchModifierEntry(CetchUpObject cetchUpObject, CetchModifier cetchModifier, CetchUpObject[] referenceObjects = null)
        {
            this.cetchUpObject = cetchUpObject;
            this.cetchModifier = cetchModifier;
            this.referenceObjects = referenceObjects;
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

        /// <summary>
        /// Reroll all rolls inside the <see cref="CetchUp.CetchModifier"/>
        /// </summary>
        public void Reroll()
        {
            foreach(ICetchLine line in CetchModifier.Lines)
            {
                line.Reroll(this);
            }
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