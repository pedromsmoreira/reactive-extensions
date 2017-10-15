using System;

namespace ReactiveExtensionsSequences
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Reactive Extensions");

            var simpleSequence = new SimpleSequence();

            simpleSequence.Simple();

            simpleSequence.Create();

            simpleSequence.Timer();

            simpleSequence.ColdObservable();
            
            simpleSequence.HotObservable();

            Console.ReadLine();
        }
    }
}