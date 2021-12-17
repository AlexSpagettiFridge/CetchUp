using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    internal class EEvariable : IEquationElement
    {
        public string name;
        public bool isNegative;

        public EEvariable() { }

        public EEvariable(string name, bool isNegative)
        {
            this.name = name;
            this.isNegative = isNegative;
        }

        public void Init(string line, ref List<string> depedencies)
        {
            Init(line);
            depedencies.Add(name);
        }

        public void Init(string line)
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

        public object Clone() => new EEvariable(name, isNegative);

        public override string ToString()
        {
            return (isNegative ? "-" : "") + name;
        }
    }
}