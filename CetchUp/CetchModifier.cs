using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using CetchUp.CetchLines;

namespace CetchUp
{
    public class CetchModifier
    {
        private List<ICetchLine> lines = new List<ICetchLine>();

        public CetchModifier(string genString)
        {
            if (Regex.IsMatch(genString, @"^[a-zA-Z0-9\/]*\.cetch$"))
            {
                StreamReader reader = new StreamReader(genString);
                Populate(reader.ReadToEnd());
            }
            else
            {
                Populate(genString);
            }
        }

        private void Populate(string cetchData)
        {
            cetchData = cetchData.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");

            lines = GetLinesFromCetchData(ref cetchData);
        }

        public void ModifyByValue(float value)
        {
            foreach (ICetchLine line in lines)
            {
                if (line is EquationLine)
                {
                    ((EquationLine)line).ModifyByValue(value);
                }
            }
        }

        private List<ICetchLine> GetLinesFromCetchData(ref string cetchData)
        {
            List<ICetchLine> result = new List<ICetchLine>();
            int index = 0;
            while ((index = cetchData.IndexOf(';')) != -1)
            {
                string line = cetchData.Substring(0, index + 1);
                cetchData = cetchData.Substring(index + 1);

                if (Regex.IsMatch(line, "^end$"))
                {
                    return result;
                }
                if (Regex.IsMatch(line, "^if:.*[<>=]{1,2}.*$"))
                {
                    result.Add(new ConditionLine(line, GetLinesFromCetchData(ref cetchData)));
                    continue;
                }
                if (Regex.IsMatch(line, "^.*[=%].*$"))
                {
                    result.Add(new EquationLine(line, this));
                    continue;
                }
            }
            return result;
        }

        public void ModifyCetchObject(CetchModifierEntry cetchModifierEntry)
        {
            foreach (ICetchLine line in lines)
            {
                line.JoinObject(cetchModifierEntry);
            }
        }

        public void RemoveFromCetchObject(CetchModifierEntry cetchModifierEntry)
        {
            foreach (ICetchLine line in lines)
            {
                line.Remove(cetchModifierEntry);
            }
        }

        public void TryShorten()
        {
            foreach (ICetchLine line in lines)
            {
                if (line is EquationLine)
                {
                    ((EquationLine)line).TryShorten();
                }
            }
        }

        public override string ToString()
        {
            string result = "";
            foreach (ICetchLine line in lines)
            {
                result += $"{line.ToString()}";
                if (line != lines[lines.Count - 1]) { result += "\n"; }
            }
            return result;
        }
    }
}