using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    public struct EEvariable : IEquationElement
    {
        public string name;
        public bool isNegative;

        public EEvariable(string line, ref List<string> depedencies) : this(line)
        {
            depedencies.Add(name);
        }

        public EEvariable(string line)
        {
            isNegative = Regex.IsMatch(line, "^-.+$");
            if (isNegative) { line = line.Substring(1); }
            name = line;
        }

        public float GetValue()
        {
            throw new System.NotImplementedException();
        }

        public float GetValue(CetchModifierEntry cetchModifierEntry)
        {
            if (name.StartsWith("#"))
            {
                return cetchModifierEntry.GetValue(name) * (isNegative ? -1 : 1);
            }
            return cetchModifierEntry.CetchUpObject.GetValue(name) * (isNegative ? -1 : 1);
        }

        public override string ToString()
        {
            return (isNegative ? "-" : "") +name;
        }
    }
}