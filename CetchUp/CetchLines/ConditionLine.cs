using System.Collections.Generic;
using System.Text.RegularExpressions;
using CetchUp.EquationElements;

namespace CetchUp.CetchLines
{
    internal class ConditionLine : ScopeLine, ICetchLine
    {
        private List<Condition> conditions = new List<Condition>();
        private List<string> dependencies = new List<string>();

        public ConditionLine(string line, List<ICetchLine> lines) : base(lines)
        {
            line = line.Substring(3);
            foreach (string conditionString in Regex.Split(line, "&&"))
            {
                conditions.Add(new Condition(conditionString, ref dependencies));
            }
        }

        public void JoinObject(CetchModifierEntry cetchModifierEntry)
        {
            foreach (CetchValue dependency in GetDependentValues(cetchModifierEntry))
            {
                dependency.changed += OnRelevantValueChanged;
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

        private List<CetchValue> GetDependentValues(CetchModifierEntry cetchModifierEntry)
        {
            List<CetchValue> result = new List<CetchValue>();
            foreach (string dep in dependencies)
            {
                if (dep.StartsWith("#"))
                {
                    result.Add(cetchModifierEntry.GetCetchValue(dep));
                    continue;
                }
                result.Add(cetchModifierEntry.CetchUpObject.GetCetchValue(dep));
            }
            return result;
        }

        public override string ToString()
        {
            string result = "";
            bool firstCondition = true;
            foreach (Condition condition in conditions)
            {
                string startString = "and  ";
                if (firstCondition) { startString = "When "; }
                result += $"{startString}{condition.ToString()}\n";
            }
            foreach (ICetchLine line in InnerLines)
            {
                result += $"     {line.ToString()}";
                if (line != InnerLines[InnerLines.Count - 1]) { result += "\n"; }
            }
            return result;
        }
    }
}