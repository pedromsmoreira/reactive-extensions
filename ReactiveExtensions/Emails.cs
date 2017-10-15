namespace ReactiveExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive.Linq;

    public class Emails
    {
        public int Seconds { get; set; } = 0;

        public void ReceiveEmails()
        {
            var myInbox = this.ProduceEmails(100000).ToObservable();

            var getMailsEveryThreeSeconds = myInbox.Buffer(TimeSpan.FromSeconds(1));

            var observer = getMailsEveryThreeSeconds.Subscribe(emails =>
            {
                var stopWatch = new Stopwatch();

                Console.WriteLine($"You have {emails.Count} emails to read.");

                stopWatch.Start();
                foreach (var email in emails)
                {
                    Console.WriteLine($"Email Id: {email.Id} | Name: {email.Name}");
                }
                stopWatch.Stop();

                this.Seconds += stopWatch.Elapsed.Seconds;
            });

            Console.WriteLine($"Took: {this.Seconds} seconds");
            observer.Dispose();
        }

        public IReadOnlyList<Email> ProduceEmails(int quantity)
        {
            var emails = new List<Email>();

            for (int i = 0; i < quantity; i++)
            {
                emails.Add(new Email
                {
                    Id = i,
                    Name = $"Email{i}"
                });
            }

            return emails;
        }
    }

    public class Email
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public event EventHandler<NameChangedEventArgs> OnNameChanged;
    }

    public class NameChangedEventArgs
    {
        public Emails Email { get; set; }
    }
}