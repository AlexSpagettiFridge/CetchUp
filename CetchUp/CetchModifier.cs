using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using CetchUp.CetchLines;
using CetchUp.EquationElements;
using System.Collections;

namespace CetchUp
{
    /// <summary>
    /// A CetchModifier is created from a .cetch file. And contains instruction on how a CetchUpObject should be modified.
    /// It can be applied to a CetchUpObject via <see cref="CetchUpObject.ApplyModifier(CetchModifier)"/>.
    /// </summary>
    public class CetchModifier
    {
        /// <summary>
        /// Reflects the lines read from the CetchLine.
        /// </summary>
        private List<ICetchLine> lines = new List<ICetchLine>();

        public CetchModifier()
        {

        }

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

        /// <summary>
        /// Fills the CetchModifer with Information.
        /// </summary>
        /// <param name="cetchData">A string as in plain Text from a cetch file</param>
        private void Populate(string cetchData)
        {
            cetchData = cetchData.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");

            lines = GetLinesFromCetchData(ref cetchData);
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
                    result.Add(new EquationLine(line));
                    continue;
                }
            }
            return result;
        }

        /// <summary>
        /// Multiplies every element inside the CetchModifier by a float.
        /// </summary>
        /// <param name="value">The factor everything should be multiplied with.</param>
        public void MultiplyByValue(float value)
        {
            foreach (ICetchLine line in lines)
            {
                line.ModifyByValue(value);
            }
        }

        /// <summary>
        /// Modifies the given cetchModifierEntry.
        /// This will apply every <see cref="lines">line</see> inside this CetchModifier.
        /// </summary>
        /// <param name="cetchModifierEntry">The CetchModifierEntry that should be modified</param>
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

        public static CetchModifier operator +(CetchModifier a, CetchModifier b)
        {
            CetchModifier c = new CetchModifier();
            List<ConditionLine> conditionLines = new List<ConditionLine>();
            Dictionary<string, List<EEequation>> variableSpecificEquations = new Dictionary<string, List<EEequation>>();
            Dictionary<string, List<EEequation>> variableSpecificModEquations = new Dictionary<string, List<EEequation>>();
            ICetchLine[] totalLines = new ICetchLine[a.lines.Count + b.lines.Count];
            a.lines.CopyTo(totalLines);
            b.lines.CopyTo(totalLines, a.lines.Count);
            foreach (ICetchLine line in totalLines)
            {
                if (line is ConditionLine)
                {
                    conditionLines.Add((ConditionLine)line.Clone());
                }
                if (line is EquationLine)
                {
                    EquationLine equationLine = (EquationLine)line;
                    string variableName = equationLine.ModifiedValue;
                    Dictionary<string, List<EEequation>> variableList;
                    if (!equationLine.IsMultiplier)
                    {
                        variableList = variableSpecificEquations;
                    }
                    else
                    {
                        variableList = variableSpecificModEquations;
                    }
                    if (!variableList.ContainsKey(variableName))
                    {
                        variableList.Add(variableName, new List<EEequation>());
                    }
                    variableList[variableName].Add(equationLine.Equation);
                }
            }
            Dictionary<string, List<EEequation>> varList = variableSpecificEquations;
            while (true)
            {
                foreach (string specificVariable in varList.Keys)
                {
                    List<EEequation> equationLines = varList[specificVariable];
                    if (equationLines.Count == 1)
                    {
                        c.lines.Add(new EquationLine(specificVariable, equationLines[0], equationLines[0].GetAllDependencies(), true));
                        continue;
                    }
                    ArrayList allEquations = new ArrayList();
                    List<string> dependencies = new List<string>();
                    for (int i = 0; i < equationLines.Count; i++)
                    {
                        if (i != 0)
                        {
                            allEquations.Add(new EEsymbol('+'));
                        }
                        allEquations.Add(equationLines[i]);
                    }
                    EEequation totalEquation = new EEequation(allEquations);
                    //totalEquation.TryShorten();
                    c.lines.Add(new EquationLine(specificVariable, totalEquation, dependencies, varList == variableSpecificModEquations));
                }
                if (varList == variableSpecificModEquations) { break; }
                varList = variableSpecificModEquations;
            }
            return c;
        }
    }
}