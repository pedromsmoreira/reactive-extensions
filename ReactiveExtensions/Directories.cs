namespace ReactiveExtensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    public class Directories
    {
        private IDisposable observer;

        public void Execute(string path)
        {
            Console.WriteLine($"Started At: ");
            var observable = Observable.Interval(TimeSpan.FromSeconds(1), Scheduler.Default)
                .Zip(GetAllDirectories(path)
                    .ToObservable(Scheduler.Default)
                    .Buffer(1000), (a, b) => b);

            observer = observable.Subscribe(folder =>
            {
                foreach (var f in folder)
                {
                    Console.WriteLine($"directory: {f}");
                }
            });

            observer.Dispose();
        }

        private static IEnumerable<string> GetAllDirectories(string path)
        {
            string[] subdirs = null;

            try
            {
                subdirs = Directory.GetDirectories(path);
            }
            catch (IOException)
            {
            }

            if (subdirs != null)
            {
                foreach (var subdir in subdirs)
                {
                    yield return subdir;

                    foreach (var grandchild in GetAllDirectories(subdir))
                    {
                        yield return grandchild;
                    }
                }
            }
        }
    }
}