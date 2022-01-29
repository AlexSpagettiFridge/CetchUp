using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    internal class EEroll : IEquationElement
    {
        private int diceAmount, diceSize;
        private int? rollResult = null;

        public EEroll() { }

        internal EEroll(int diceAmount, int diceSize)
        {
            this.diceAmount = diceAmount;
            this.diceSize = diceSize;
        }

        public void Init(string line)
        {
            Match match = Regex.Match(line, "([0-9]+)d([0-9]+)");
            diceAmount = int.Parse(match.Groups[1].Value);
            diceSize = int.Parse(match.Groups[2].Value);
        }

        public void Init(string line, ref List<string> dependencies) => Init(line);

        public object Clone() => new EEroll(diceAmount, diceSize);

        public float GetValue()
        {
            if (rollResult == null)
            {
                Reroll();
            }
            return (int)rollResult;
        }

        public float GetValue(CetchModifierEntry cetchModifierEntry) => GetValue();

        public void Reroll()
        {
            rollResult = 0;
            for (int i = 0; i < diceAmount; i++)
            {
                rollResult += new Random().Next(1, diceSize);
            }
        }

        public override string ToString()
        {
            return $"{diceAmount}d{diceSize}";
        }
    }
}