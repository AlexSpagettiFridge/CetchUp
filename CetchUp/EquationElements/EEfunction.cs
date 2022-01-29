using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    internal class EEfunction : IEquationElement
    {
        private FunctionType functionType;
        private IEquationElement innerElement;

        public EEfunction() { }

        internal EEfunction(FunctionType functionType, IEquationElement innerElement)
        {
            this.functionType = functionType;
            this.innerElement = innerElement;
        }

        public void Init(string line, ref List<string> dependencies)
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

        public void Init(string line)
        {
            List<string> unnecessaryList = new List<string>();
            Init(line, ref unnecessaryList);
        }

        public object Clone()
        {
            return new EEfunction(functionType, (IEquationElement)innerElement.Clone());
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

        public void Reroll() { }

        public enum FunctionType
        {
            Round, Floor, Ceil
        }
    }
}