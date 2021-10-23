using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace CetchUp
{
    public class CetchModifier
    {
        CetchUpObject cetchUpObject;
        private List<ICetchLine> lines = new List<ICetchLine>();

        public CetchModifier(CetchUpObject cetchUpObject, string filePath)
        {
            this.cetchUpObject = cetchUpObject;
            StreamReader reader = new StreamReader(filePath);
            Populate(reader.ReadToEnd());
        }

        private void Populate(string cetchData)
        {
            cetchData = cetchData.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");
            Regex equationReg = new Regex("^.*[=%].*$");
            Regex initReg = new Regex("^#.*$");
            int index = 0;
            while ((index = cetchData.IndexOf(';')) != -1)
            {
                string line = cetchData.Substring(0, index + 1);
                cetchData = cetchData.Substring(index + 1);

                if (initReg.IsMatch(line))
                {
                    lines.Add(new InitializeLine(line));
                    continue;
                }
                if (equationReg.IsMatch(line))
                {
                    lines.Add(new EquationLine(line));
                    continue;
                }
            }
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