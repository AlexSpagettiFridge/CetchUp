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
        private static readonly Regex equationReg = new Regex("^.*[=%].*$"), initReg = new Regex("^#.*$");
        private static readonly Regex endReg = new Regex("^end$"), ifReg = new Regex("^if:.*[<>=]{1,2}.*$");

        public CetchModifier(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            Populate(reader.ReadToEnd());
        }

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

                if (endReg.IsMatch(line))
                {
                    return result;
                }
                if (ifReg.IsMatch(line))
                {
                    result.Add(new ConditionLine(line, GetLinesFromCetchData(ref cetchData)));
                    continue;
                }
                if (initReg.IsMatch(line))
                {
                    result.Add(new InitializeLine(line));
                    continue;
                }
                if (equationReg.IsMatch(line))
                {
                    result.Add(new EquationLine(line));
                    continue;
                }
            }
            return result;
        }

        public void ModifyCetchObject(CetchUpObject cetchUpObject)
        {
            foreach (ICetchLine line in lines)
            {
                line.JoinObject(cetchUpObject);
            }
        }

        public void RemoveFromCetchObject(CetchUpObject cetchUpObject)
        {
            foreach (ICetchLine line in lines)
            {
                line.Remove(cetchUpObject);
            }
        }
    }
}