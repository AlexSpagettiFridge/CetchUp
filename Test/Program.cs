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

            cetchUpObject.ApplyModifier(repository["Rolls"]);
            Console.WriteLine(cetchUpObject.GetValue("x"));
            
            CetchUpObject cetchUpObject2 = new CetchUpObject();
            cetchUpObject2.ApplyModifier(repository["References"],new CetchUpObject[] {cetchUpObject});
            Console.WriteLine(cetchUpObject2.GetValue("y"));

        }
    }
}
