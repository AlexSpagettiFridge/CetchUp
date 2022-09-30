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
    public class CetchModifier : ICloneable
    {
        /// <summary>
        /// Reflects the lines read from the CetchLine.
        /// </summary>
        internal List<ICetchLine> Lines = new List<ICetchLine>();

        public string Name = null;

        public CetchModifier()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref ="CetchModifier"/>
        /// </summary>
        /// <param name="genString">
        /// The genString can either be a raw Cetch Script or a path to an .cetch file.
        /// </param>
        public CetchModifier(string genString)
        {
            if (Regex.IsMatch(genString, @"^[a-zA-Z0-9\/\\]*\.cetch$"))
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
            cetchData = cetchData.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");

            Lines = GetLinesFromCetchData(ref cetchData);
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
                if (Regex.IsMatch(line, "^N:.*$"))
                {
                    Name = line.Substring(2,line.Length-3);
                    continue;
                }
                if (Regex.IsMatch(line, "^if:.*[<>=]{1,2}.*$"))
                {
                    result.Add(new ConditionLine(line, GetLinesFromCetchData(ref cetchData)));
                    continue;
                }
                if (Regex.IsMatch(line, "^.*[=%mMoO].*$"))
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
            foreach (ICetchLine line in Lines)
            {
                line.ModifyByValue(value);
            }
        }

        /// <summary>
        /// Modifies the given cetchModifierEntry.
        /// This will apply every <see cref="Lines">line</see> inside this CetchModifier.
        /// </summary>
        /// <param name="cetchModifierEntry">The CetchModifierEntry that should be modified</param>
        public void ModifyCetchObject(CetchModifierEntry cetchModifierEntry)
        {
            foreach (ICetchLine line in Lines)
            {
                line.JoinObject(cetchModifierEntry);
            }
        }

        /// <summary>
        /// Replaces all instances of the variable <paramref name="varName"/> with the constant <paramref name="value"/>.
        /// </summary>
        /// <param name="varName">The name of the local variable that should be replaced.</param>
        /// <param name="value">The constant value the variables should be replaced with.</param>
        public void InsertVariable(string varName, float value)
        {
            foreach (ICetchLine line in Lines)
            {
                line.InsertVariable(varName, value);
            }
        }

        public void RemoveFromCetchObject(CetchModifierEntry cetchModifierEntry)
        {
            foreach (ICetchLine line in Lines)
            {
                line.Remove(cetchModifierEntry);
            }
        }

        public void TryShorten()
        {
            foreach (ICetchLine line in Lines)
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
            foreach (ICetchLine line in Lines)
            {
                result += $"{line.ToString()}";
                if (line != Lines[Lines.Count - 1]) { result += "\n"; }
            }
            return result;
        }

        public object Clone()
        {
            CetchModifier clone = new CetchModifier();
            clone.Name = Name;
            foreach (ICetchLine line in Lines)
            {
                clone.Lines.Add((ICetchLine)line.Clone());
            }
            return clone;
        }

        public static CetchModifier operator +(CetchModifier a, CetchModifier b)
        {
            CetchModifier c = new CetchModifier();
            List<ConditionLine> conditionLines = new List<ConditionLine>();

            Dictionary<CetchValue.ValuePart, Dictionary<string, List<EEequation>>> variableSpecificEquations = new Dictionary<CetchValue.ValuePart, Dictionary<string, List<EEequation>>>();
            foreach (CetchValue.ValuePart valuePart in Enum.GetValues(typeof(CetchValue.ValuePart)))
            {
                variableSpecificEquations[valuePart] = new Dictionary<string, List<EEequation>>();
            }

            ICetchLine[] totalLines = new ICetchLine[a.Lines.Count + b.Lines.Count];
            a.Lines.CopyTo(totalLines);
            b.Lines.CopyTo(totalLines, a.Lines.Count);
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
                    variableList = variableSpecificEquations[equationLine.ValuePart];

                    if (!variableList.ContainsKey(variableName))
                    {
                        variableList.Add(variableName, new List<EEequation>());
                    }
                    variableList[variableName].Add(equationLine.Equation);
                }
            }

            foreach (CetchValue.ValuePart valuePart in Enum.GetValues(typeof(CetchValue.ValuePart)))
            {
                Dictionary<string, List<EEequation>> varList = variableSpecificEquations[valuePart];
                foreach (string specificVariable in varList.Keys)
                {
                    List<EEequation> equationLines = varList[specificVariable];
                    if (equationLines.Count == 1)
                    {
                        c.Lines.Add(new EquationLine(specificVariable, equationLines[0], equationLines[0].GetAllDependencies(), valuePart));
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
                    totalEquation.TryShorten();
                    c.Lines.Add(new EquationLine(specificVariable, totalEquation, dependencies, valuePart));
                }
            }
            return c;
        }


    }
}