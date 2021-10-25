using System;

namespace CetchUp.CetchLines
{
    internal class InitializeLine : ICetchLine
    {
        private string variableName;
        private float defaultValue, defaultModifier;

        public InitializeLine(string cetchLine)
        {
            PopulateFromCetchLine(cetchLine);
        }

        public void PopulateFromCetchLine(string line)
        {
            int eqIndex = line.IndexOf('=');
            int quIndex = line.IndexOf('?');
            int scIndex = line.IndexOf(';');

            variableName = line.Substring(1, eqIndex - 1);
            defaultValue = float.Parse(line.Substring(eqIndex + 1, (quIndex != -1 ? quIndex : scIndex) - (eqIndex + 1)));

            defaultModifier = 1;
            if (quIndex != -1)
            {
                defaultModifier = float.Parse(line.Substring(quIndex + 1, scIndex - (quIndex + 1)));
            }
        }

        public void JoinObject(CetchUpObject cetchUpObject)
        {
            cetchUpObject.AddNewValue(variableName, defaultValue, defaultModifier);
        }

        public void Remove(CetchUpObject cetchUpObject) { }
    }
}