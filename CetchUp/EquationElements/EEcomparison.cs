using System.Text.RegularExpressions;

namespace CetchUp.EquationElements
{
    internal class EEcomparison : IEquationElement
    {
        public ComparisonType comparisonType;
        public bool isInclusive;

        public EEcomparison(string snippet)
        {
            isInclusive = new Regex("^.=").IsMatch(snippet);
            if (snippet == "=")
            {
                comparisonType = ComparisonType.equal;
                return;
            }
            comparisonType = new Regex("^>.?").IsMatch(snippet) ? ComparisonType.greater : ComparisonType.smaller;
        }

        public enum ComparisonType
        {
            equal, greater, smaller
        }
    }
}