using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace CetchUp.EquationElements
{
    internal static class EquationHelper
    {
        public static IEquationElement CreateEquationElementFromLine(string line, ref List<string> dependencies)
        {
            if (Regex.IsMatch(line, @"^\(.*\)$"))
            {
                return new EEequation(line.Substring(1, line.Length - 2), ref dependencies);
            }
            if (Regex.IsMatch(line, "^[A-z_]*$"))
            {
                return new EEvariable(line);
            }
            if (Regex.IsMatch(line, @"^[0-9\.]*$"))
            {
                return new EEconstant(line);
            }
            if (Regex.IsMatch(line, @"^[+\-*\/]$"))
            {
                return new EEsymbol(line);
            }
            return new EEequation(line, ref dependencies);
        }
    }
}