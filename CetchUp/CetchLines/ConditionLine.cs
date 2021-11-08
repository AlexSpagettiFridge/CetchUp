using System.Collections.Generic;
using CetchUp.EquationElements;

namespace CetchUp.CetchLines
{
    internal class ConditionLine : ScopeLine, ICetchLine
    {
        private List<Condition> conditions = new List<Condition>();

        public ConditionLine(string line, List<ICetchLine> lines, CetchModifier cetchModifier) : base(lines)
        {
            line = line.Substring(3);
            while (true)
            {
                int nextStop = line.IndexOfAny(new char[] { ';', '&' });
                conditions.Add(new Condition(line.Substring(0, nextStop), cetchModifier));
                line = line.Substring(nextStop);
                if (line.StartsWith("&&"))
                {
                    line = line.Substring(0, 2);
                    continue;
                }
                break;
            }
        }

        public void JoinObject(CetchModifierEntry cetchModifierEntry)
        {
            foreach (Condition condition in conditions)
            {
                AddEventToValue(cetchModifierEntry, condition.firstValue);
                AddEventToValue(cetchModifierEntry, condition.secondValue);
            }
            CheckConditionsForObject(cetchModifierEntry);
        }

        public void CheckConditionsForObject(CetchModifierEntry cetchModifierEntry)
        {
            bool conditionsMet = true;
            foreach (Condition condition in conditions)
            {

                if (!condition.IsConditionMet(cetchModifierEntry)) { conditionsMet = false; }
            }

            if (conditionsMet)
            {
                JoinInnerLines(cetchModifierEntry);
            }
            else
            {
                RemoveInnerLines(cetchModifierEntry);
            }
        }

        private void AddEventToValue(CetchModifierEntry cetchModifierEntry, IEquationElement valueElement)
        {
            if (valueElement is EEvariable)
            {
                cetchModifierEntry.CetchUpObject.GetCetchValue(((EEvariable)valueElement).variableName).changed
                += OnRelevantValueChanged;
            }
        }

        public void Remove(CetchModifierEntry cetchModifierEntry)
        {
            RemoveInnerLines(cetchModifierEntry);
        }

        public void OnRelevantValueChanged(object sender, CetchValue.ChangedEventArgs args)
        {
            CheckConditionsForObject(args.cetchModifierEntry);
        }
    }
}