namespace ReactiveExtensions
{
    using System;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var directories = new Directories();

            //directories.Execute(@"C:\Git");

            var emails = new Emails();

            emails.ReceiveEmails();

            Console.ReadLine();
        }
    }
}