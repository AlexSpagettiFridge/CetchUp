using System.Text.RegularExpressions;
using CetchUp.EquationElements;
using System;
using System.Collections.Generic;

namespace CetchUp
{
    internal class Condition
    {
        public IEquationElement firstValue, secondValue;
        public ComparisonType comparer;

        public Condition(string line, ref List<string> dependencies)
        {
            switch (Regex.Match(line, "<=|>=|==|<|>").Value)
            {
                case ">=": comparer = ComparisonType.GreaterEqual; break;
                case "<=": comparer = ComparisonType.SmallerEqual; break;
                case ">": comparer = ComparisonType.Greater; break;
                case "<": comparer = ComparisonType.Smaller; break;
                default: comparer = ComparisonType.Equal; break;
            }
            string[] parts = Regex.Split(line, "<=|>=|==|<|>");
            if (parts.Length != 2)
            {
                throw new ArgumentException($"Cannot parse \"{line}\" as a comparison.\nIt has too many comparison symbols.");
            }
            firstValue = EquationHelper.CreateEquationElementFromLine(parts[0], ref dependencies);
            secondValue = EquationHelper.CreateEquationElementFromLine(parts[1], ref dependencies);
        }

        public bool IsConditionMet(CetchModifierEntry cetchModifierEntry)
        {
            float a = firstValue.GetValue(cetchModifierEntry);
            float b = secondValue.GetValue(cetchModifierEntry);
            switch (comparer)
            {
                case ComparisonType.GreaterEqual: return a >= b;
                case ComparisonType.SmallerEqual: return a <= b;
                case ComparisonType.Greater: return a > b;
                case ComparisonType.Smaller: return a < b;
                default: return a == b;
            }
        }

        public enum ComparisonType
        {
            Greater, Smaller, Equal, GreaterEqual, SmallerEqual
        }
    }
}