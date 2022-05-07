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
            cetchUpObject.ValueChanged+=OnValueChanged;

            CetchModifierRepository repository = new CetchModifierRepository();
            repository.AddDirectory("testfiles/");

            cetchUpObject.ApplyModifier(repository["BaseValueTest"]);
            cetchUpObject.GetCetchValue("bla").BaseValue += 3;
            Console.WriteLine(cetchUpObject.GetValue("yee"));
        }

        private static void OnValueChanged(object sender,CetchValueCollection.ValueChangedEventArgs args)
        {
            Console.WriteLine("yeah");
        }
    }
}
