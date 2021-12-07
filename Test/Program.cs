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

            Console.WriteLine("--ShortenEquation.cetch");
            Console.WriteLine(repository["ShortenEquations"]);
            Console.WriteLine("--ShortenEquation.cetch (shortened)");
            repository["ShortenEquations"].TryShorten();
            Console.WriteLine(repository["ShortenEquations"]);
            Console.WriteLine("--ShortenEquation.cetch (modified by 0.5f)");
            repository["ShortenEquations"].MultiplyByValue(0.5f);
            Console.WriteLine(repository["ShortenEquations"]);

            Console.WriteLine("--Yep.cetch & Yep.cetch");
            Console.WriteLine(repository["Addition.Yep"]);
            Console.WriteLine(repository["Addition.Yip"]);
            Console.WriteLine("--Yep.cetch & Yip.cetch (combined)");
            Console.WriteLine(repository["Addition.Yep"] + repository["Addition.Yip"]);
            Console.WriteLine("--3x Worms (combined)");
            Console.WriteLine(repository["Addition.Worms"] + repository["Addition.Worms"] + repository["Addition.Worms"]);
            CetchModifier superWorms = new CetchModifier();
            for(int i=0;i<3;i++){
                superWorms+=repository["Addition.Worms"];
            }
            Console.WriteLine("--3x Worms (combined differently)");
            Console.WriteLine(repository["Addition.Worms"] + repository["Addition.Worms"] + repository["Addition.Worms"]);
        }
        private static void WriteCurrentStats(CetchUpObject cetchUpObject)
        {
            Console.WriteLine($"#----Current-Stats----#");
            Console.WriteLine($"maxHealth: {cetchUpObject.GetValue("maxHealth")}");
            Console.WriteLine($"smellResistance: {cetchUpObject.GetValue("smellResistance")}");
            Console.WriteLine($"endurance: {cetchUpObject.GetValue("endurance")}");
            Console.WriteLine($"bigBoyCetificate: {cetchUpObject.GetValue("bigBoyCetificate")}");
            Console.WriteLine($"wumbleSnatch: {cetchUpObject.GetValue("wumbleSnatch")}");
        }
    }
}
