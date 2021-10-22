using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CetchUp
{
    public class CetchUpObject
    {
        private Dictionary<string, CetchValue> values = new Dictionary<string, CetchValue>();
        private List<CetchModifier> modifiers = new List<CetchModifier>();

        public CetchUpObject(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            Populate(reader.ReadToEnd());
        }

        private void Populate(string cetchData)
        {
            Regex equationReg = new Regex("/[=%]");
            int index = 0;
            while ((index = cetchData.IndexOf(';')) != -1)
            {
                string line = cetchData.Substring(0, index + 1);
                cetchData = cetchData.Substring(index + 1);
                if (equationReg.IsMatch(line)){
                    Console.WriteLine(line);
                }
            }
        }

        public float GetValue(string valueName)
        {
            try
            {
                return values[valueName].Total;
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception($"The CetchUpObject does not contain {e.Data["Key"]}");
            }
        }

        internal CetchValue GetCetchValue(string valueName)
        {
            return values[valueName];
        }
    }
}