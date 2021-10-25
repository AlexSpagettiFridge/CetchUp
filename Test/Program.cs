using System;
using CetchUp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Doing it...");
            CetchUpObject cetchUpObject = new CetchUpObject();

            CetchModifierRepository repository = new CetchModifierRepository();
            repository.AddDirectory("testfiles/");

            cetchUpObject.ApplyModifier(repository["Header"]);
            WriteCurrentStats(cetchUpObject);
            cetchUpObject.ApplyModifier(repository["Bla"]);
            WriteCurrentStats(cetchUpObject);
            cetchUpObject.RemoveModifier(repository["Bla"]);
            WriteCurrentStats(cetchUpObject);

            Console.WriteLine("the End");
        }
        private static void WriteCurrentStats(CetchUpObject cetchUpObject)
        {
            Console.WriteLine($"#----Current-Stats----#");
            Console.WriteLine($"maxHealth: {cetchUpObject.GetValue("maxHealth")}");
            Console.WriteLine($"smellResistance: {cetchUpObject.GetValue("smellResistance")}");
            Console.WriteLine($"endurance: {cetchUpObject.GetValue("endurance")}");
            Console.WriteLine($"bigBoyCetificate: {cetchUpObject.GetValue("bigBoyCetificate")}");
        }
    }
}
