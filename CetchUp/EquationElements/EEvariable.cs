using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace CetchUp.EquationElements
{
    internal class EEvariable : IEquationElement
    {
        public string name;
        public bool isNegative;
        public int referenceId = -1;

        public EEvariable() { }

        public EEvariable(string name, bool isNegative, int referenceId)
        {
            this.name = name;
            this.isNegative = isNegative;
            this.referenceId = referenceId;
        }

        public void Init(string line, ref List<string> depedencies)
        {
            Init(line);
            depedencies.Add(name);
        }

        public void Init(string line)
        {
            referenceId = -1;
            isNegative = Regex.IsMatch(line, "^-.+$");
            if (isNegative) { line = line.Substring(1); }
            Match match = Regex.Match(line, @"^(\@([0-9])*\.)?(.*)$");
            if (match.Groups[2].Success)
            {
                referenceId = int.Parse(match.Groups[2].Value);
            }
            name = match.Groups[3].Value;
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
            if (referenceId > -1)
            {
                return cetchModifierEntry.ReferenceObjects[referenceId].GetValue(name) * (isNegative ? -1 : 1);
            }
            return cetchModifierEntry.CetchUpObject.GetValue(name) * (isNegative ? -1 : 1);
        }

        public object Clone() => new EEvariable(name, isNegative, referenceId);

        public override string ToString()
        {
            return (isNegative ? "-" : "") + name;
        }

        public void Reroll() { }
    }
}