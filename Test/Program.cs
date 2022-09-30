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

            cetchUpObject.ApplyModifier(repository["MinMaxA"]);
            Console.WriteLine(cetchUpObject.GetValue("oink"));
            Console.WriteLine(cetchUpObject.GetValue("hitPoints"));
            Console.WriteLine(cetchUpObject.GetCetchValue("hitPoints").Max);
            cetchUpObject.ApplyModifier(repository["MinMaxB"]);
            Console.WriteLine(cetchUpObject.GetValue("oink"));
            cetchUpObject.GetCetchValue("oink").Total -= 15;
            Console.WriteLine(cetchUpObject.GetValue("oink"));
            cetchUpObject.TryRemoveModifier(repository["MinMaxA"]);
            Console.WriteLine(cetchUpObject.GetValue("oink"));
            cetchUpObject.GetCetchValue("oink").Total += 20;
            Console.WriteLine(cetchUpObject.GetValue("oink"));
            foreach (ValueModEntry entry in cetchUpObject.GetCetchValue("oink").GetAllValueModEntries())
            {
                Console.WriteLine($"{entry.ModName} : {entry.EquationString}");
            }

        }

    }
}
