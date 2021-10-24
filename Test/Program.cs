using System;
using CetchUp;

namespace Test
{
    class Program
    {
        public static CetchUpObject cetchUpObject;

        static void Main(string[] args)
        {
            Console.WriteLine("Doing it...");
            cetchUpObject = new CetchUpObject();

            CetchModifier header = new CetchModifier(cetchUpObject, "testfiles/Header.cetch");
            CetchModifier bla = new CetchModifier(cetchUpObject, "testfiles/Bla.cetch");
            CetchUpObject moo = cetchUpObject;

            cetchUpObject.ApplyModifier(header);
            WriteCurrentStats();
            cetchUpObject.ApplyModifier(bla);
            WriteCurrentStats();
            cetchUpObject.RemoveModifier(bla);
            WriteCurrentStats();
            
            Console.WriteLine("the End");
        }
        private static void WriteCurrentStats()
        {
            Console.WriteLine($"#----Current-Stats----#");
            Console.WriteLine($"maxHealth: {cetchUpObject.GetValue("maxHealth")}");
            Console.WriteLine($"smellResistance: {cetchUpObject.GetValue("smellResistance")}");
            Console.WriteLine($"endurance: {cetchUpObject.GetValue("endurance")}");
            Console.WriteLine($"bigBoyCetificate: {cetchUpObject.GetValue("bigBoyCetificate")}");
        }
    }
}
