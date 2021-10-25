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

        public void JoinInnerLines(CetchUpObject cetchUpObject)
        {
            if (!joinedObjects.Contains(cetchUpObject)) { return; }
            joinedObjects.Add(cetchUpObject);
            foreach (ICetchLine line in innerLines)
            {
                line.JoinObject(cetchUpObject);
            }
        }

        public void RemoveInnerLines(CetchUpObject cetchUpObject)
        {
            if (joinedObjects.Contains(cetchUpObject)) { return; }
            joinedObjects.Remove(cetchUpObject);
            foreach (ICetchLine line in innerLines)
            {
                line.Remove(cetchUpObject);
            }
        }
    }
}