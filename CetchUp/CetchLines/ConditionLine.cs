using System.Collections.Generic;
using System.Text.RegularExpressions;
using CetchUp.EquationElements;

namespace CetchUp.CetchLines
{
    internal class ConditionLine : ScopeLine, ICetchLine
    {
        private List<Condition> conditions = new List<Condition>();
        private List<string> depedencies = new List<string>();

        public ConditionLine(string line, List<ICetchLine> lines) : base(lines)
        {
            line = line.Substring(3);
            foreach (string conditionString in Regex.Split(line, "&&"))
            {
                conditions.Add(new Condition(conditionString, ref depedencies));
            }
        }

        public void JoinObject(CetchModifierEntry cetchModifierEntry)
        {
            foreach(string dependency in depedencies){
                if (dependency.StartsWith("#"))
                {
                    cetchModifierEntry.GetCetchValue(dependency).changed += OnRelevantValueChanged;
                }
                cetchModifierEntry.CetchUpObject.GetCetchValue(dependency).changed += OnRelevantValueChanged;
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