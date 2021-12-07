using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    public class EEfunction : IEquationElement
    {
        private FunctionType functionType;
        private IEquationElement innerElement;

        public EEfunction(string line, ref List<string> dependencies)
        {
            Match match = Regex.Match(line, @"^(Round|Floor|Ceil)\((.*)\)$");
            switch (match.Groups[1].Value)
            {
                case "Round": functionType = FunctionType.Round; break;
                case "Floor": functionType = FunctionType.Floor; break;
                case "Ceil": functionType = FunctionType.Ceil; break;
            }
            innerElement = EquationHelper.CreateEquationElementFromLine(match.Groups[2].Value, ref dependencies);
        }

        public object Clone()
        {
            throw new System.NotImplementedException();
        }

        public float GetValue()
        {
            switch (functionType)
            {
                case FunctionType.Round: return (float)Math.Round(innerElement.GetValue());
                case FunctionType.Floor: return (float)Math.Floor(innerElement.GetValue());
                case FunctionType.Ceil: return (float)Math.Ceiling(innerElement.GetValue());
                default: return 0;
            }
        }

        public float GetValue(CetchModifierEntry cetchModifierEntry)
        {
            switch (functionType)
            {
                case FunctionType.Round: return (float)Math.Round(innerElement.GetValue(cetchModifierEntry));
                case FunctionType.Floor: return (float)Math.Floor(innerElement.GetValue(cetchModifierEntry));
                case FunctionType.Ceil: return (float)Math.Ceiling(innerElement.GetValue(cetchModifierEntry));
                default: return 0;
            }
        }

        public enum FunctionType
        {
            Round, Floor, Ceil
        }
    }
}