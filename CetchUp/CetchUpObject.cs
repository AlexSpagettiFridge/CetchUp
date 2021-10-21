using System;
using System.Collection.Generic;

namespace CetchUp
{
    public class CetchUpObject
    {
        private Dictionary<string, float> values = new Dictionary<string, float>();
        private List<CetchModifier> modifiers = List<CetchModifier>();
    }
}