using System.Collections.Generic;

namespace CetchUp.CetchLines
{
    internal abstract class ScopeLine
    {
        private List<ICetchLine> innerLines = new List<ICetchLine>();
        private List<CetchUpObject> joinedObjects = new List<CetchUpObject>();

        public ScopeLine(List<ICetchLine> lines)
        {
            innerLines = lines;
        }

        public void JoinInnerLines(CetchModifierEntry cetchModifierEntry)
        {
            if (joinedObjects.Contains(cetchModifierEntry.CetchUpObject)) { return; }
            joinedObjects.Add(cetchModifierEntry.CetchUpObject);
            foreach (ICetchLine line in innerLines)
            {
                line.JoinObject(cetchModifierEntry);
            }
        }

        public void RemoveInnerLines(CetchModifierEntry cetchModifierEntry)
        {
            if (!joinedObjects.Contains(cetchModifierEntry.CetchUpObject)) { return; }
            joinedObjects.Remove(cetchModifierEntry.CetchUpObject);
            foreach (ICetchLine line in innerLines)
            {
                line.Remove(cetchModifierEntry);
            }
        }
    }
}