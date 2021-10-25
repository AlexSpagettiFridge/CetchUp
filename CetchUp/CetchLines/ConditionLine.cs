using System.Collections.Generic;
using CetchUp.EquationElements;

namespace CetchUp.CetchLines
{
    internal class ConditionLine : ScopeLine, ICetchLine
    {
        private List<Condition> conditions = new List<Condition>();

        public ConditionLine(string line, List<ICetchLine> lines) : base(lines)
        {
            line = line.Substring(3);
            while (true)
            {
                int nextStop = line.IndexOfAny(new char[] { ';', '&' });
                conditions.Add(new Condition(line.Substring(0, nextStop)));
                line = line.Substring(nextStop);
                if (line.StartsWith("&&"))
                {
                    line = line.Substring(0, 2);
                    continue;
                }
                break;
            }
        }

        public void JoinObject(CetchUpObject cetchUpObject)
        {
            foreach (Condition condition in conditions)
            {
                AddEventToValue(cetchUpObject, condition.firstValue);
                AddEventToValue(cetchUpObject, condition.secondValue);
            }
            CheckConditionsForObject(cetchUpObject);
        }

        public void CheckConditionsForObject(CetchUpObject cetchUpObject)
        {
            bool conditionsMet = true;
            foreach (Condition condition in conditions)
            {

                if (!condition.IsConditionMet(cetchUpObject)) { conditionsMet = false; }
            }

            if (conditionsMet)
            {
                JoinInnerLines(cetchUpObject);
            }
            else
            {
                RemoveInnerLines(cetchUpObject);
            }
        }

        private void AddEventToValue(CetchUpObject cetchUpObject, IEquationElement valueElement)
        {
            if (valueElement is EEvariable)
            {
                cetchUpObject.GetCetchValue(((EEvariable)valueElement).variableName).changed += OnRelevantValueChanged;
            }
        }

        public void Remove(CetchUpObject cetchUpObject)
        {
            RemoveInnerLines(cetchUpObject);
        }

        public void OnRelevantValueChanged(object sender, CetchValue.ChangedEventArgs args)
        {
            CheckConditionsForObject(args.cetchUpObject);
        }
    }
}