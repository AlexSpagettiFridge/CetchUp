using System;

namespace CetchUp
{
    public class CetchUpReferenceException : Exception
    {
        public CetchModifierEntry cetchModifierEntry;
        public string variableName;
        public int referenceId;

        public CetchUpReferenceException(CetchModifierEntry cetchModifierEntry, string variableName, int referenceId)
        {
            this.cetchModifierEntry = cetchModifierEntry;
            this.variableName = variableName;
            this.referenceId = referenceId;
        }

        public override string ToString()
        {
            return $"A CetchModifier tries to gather the variable {variableName} from reference index {referenceId}. " +
            cetchModifierEntry.ReferenceObjects==null? "No optional parameter was given to the ApplyModifer Method:":
            $"Only {cetchModifierEntry.ReferenceObjects.Length} references were given.";
        }
    }
}