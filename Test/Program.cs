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

            CetchModifierEntry test = cetchUpObject.ApplyModifier(repository["Rolls"]);
            Console.WriteLine(cetchUpObject.GetValue("x"));
            test.Reroll();
            Console.WriteLine(cetchUpObject.GetValue("x"));
            cetchUpObject.Reroll();
            Console.WriteLine(cetchUpObject.GetValue("x"));
        }
    }
}
