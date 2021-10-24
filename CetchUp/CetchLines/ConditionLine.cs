using System.Collections.Generic;
using CetchUp.EquationElements;

namespace CetchUp.CetchLines
{
    internal class ConditionLine : ICetchLine
    {
        private List<ICetchLine> innerLines = new List<ICetchLine>();
        private List<Condition> conditions = new List<Condition>();
        private List<CetchUpObject> metObject = new List<CetchUpObject>();

        public ConditionLine(string line, List<ICetchLine> lines)
        {
            innerLines = lines;
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
                if (metObject.Contains(cetchUpObject)) { return; }
                metObject.Add(cetchUpObject);
                foreach (ICetchLine line in innerLines)
                {
                    line.JoinObject(cetchUpObject);
                }
            }
            else
            {
                if (metObject.Contains(cetchUpObject))
                {
                    metObject.Remove(cetchUpObject);
                    foreach (ICetchLine line in innerLines)
                    {
                        line.Remove(cetchUpObject);
                    }
                }
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
            foreach (ICetchLine line in innerLines)
            {
                line.Remove(cetchUpObject);
            }
        }

        public void OnRelevantValueChanged(object sender, CetchValue.ChangedEventArgs args)
        {
            CheckConditionsForObject(args.cetchUpObject);
        }
    }
}