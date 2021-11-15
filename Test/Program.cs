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

            Console.WriteLine("CetchUpObject:");
            cetchUpObject.ApplyModifier(repository["Header"]);
            WriteCurrentStats(cetchUpObject);
            cetchUpObject.ApplyModifier(repository["Bla"]);
            WriteCurrentStats(cetchUpObject);
            cetchUpObject.TryRemoveModifier(repository["Bla"]);
            WriteCurrentStats(cetchUpObject);

            Console.WriteLine("Cetchy:");
            CetchUpObject cetchy = new CetchUpObject();
            cetchy.ApplyModifier(repository["Header"]);
            WriteCurrentStats(cetchy);
            cetchy.ApplyModifier(cetchUpObject.MakeModifer());
            WriteCurrentStats(cetchy);

            Console.WriteLine("--Modifier toString");
            Console.WriteLine(repository["Header"].ToString());
            Console.WriteLine(repository["Bla"].ToString());

            Console.WriteLine(repository["ShortenEquations"]);
            repository["ShortenEquations"].TryShorten();
            Console.WriteLine(repository["ShortenEquations"]);
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
