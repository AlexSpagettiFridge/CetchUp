using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    internal class EEroll : IEquationElement
    {
        private int diceAmount, diceSize;

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
            throw new System.NotImplementedException();
        }

        public float GetValue(CetchModifierEntry cetchModifierEntry)
        {
            throw new System.NotImplementedException();
        }
    }
}