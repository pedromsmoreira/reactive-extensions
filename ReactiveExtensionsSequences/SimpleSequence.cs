namespace ReactiveExtensionsSequences
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;

    public class SimpleSequence
    {
        public void Simple()
        {
            Console.WriteLine("Simple Sequence");
            var source = Observable.Range(1, 10);

            var subscription = source.Subscribe(
                x => Console.WriteLine($"OnNext: {x}"),
                ex => Console.WriteLine($"OnError: {ex.Message}"),
                () => Console.WriteLine("OnComplete"));

            Console.WriteLine("Press ENTER to unsubscribe...");
            Console.ReadLine();
            subscription.Dispose();
        }

        public void Create()
        {
            Console.WriteLine("Create Method Sequence");
            var source = Observable.Range(1, 10);

            var observer = Observer.Create<int>(
                x => Console.WriteLine($"OnNext: {x}"),
                ex => Console.WriteLine($"OnError: {ex.Message}"),
                () => Console.WriteLine("OnComplete"));

            var subscription = source.Subscribe(observer);

            Console.WriteLine("Press ENTER to unsubscribe...");
            Console.ReadLine();
            subscription.Dispose();
        }

        public void Timer()
        {
            Console.WriteLine("Timer Method Sequence");
            Console.WriteLine($"Current Time: {DateTime.UtcNow}");

            // Wait 5 seconds for first pull. Then pull every second.
            var source = Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1))
                .Timestamp();

            using (source.Subscribe(x => Console.WriteLine($"{x.Value}: {x.Timestamp}")))
            {
                Console.WriteLine("Press any key to unsubscribe");
                Console.ReadKey();
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public void ConvertEnumerableToObservableSequence()
        {
            Console.WriteLine("Convert Enumerable To Observable Sequence");

            IEnumerable<int> e = new List<int> { 1, 2, 3, 4, 5 };

            var source = e.ToObservable();

            var subscription = source.Subscribe(
                x => Console.WriteLine("OnNext: {0}", x),
                ex => Console.WriteLine("OnError: {0}", ex.Message),
                () => Console.WriteLine("OnCompleted"));

            Console.ReadKey();
        }

        public void ColdObservable()
        {
            /* Cold Observables start running upon subscription
             * the observable sequence only starts pushing data to the observers when subscriber is called.
             * The data is not shared among subscribers.
             * This is different from hot observables such as mouse move events or stock tickers which are already producing
             * values even before a subscription is active.
             * When an observer subscribes to a hot observable sequence, it will get the current value in the stream.
             * The hot observable sequence is shared among all subscribers, and each subscriber is pushed the next value in
             * the sequence.
             * For example, even if no one has subscribed to a particular stock ticker, the ticker will continue to update
             * its value based on market movement.
             * When a subscriber registers interest in this ticker, it will automatically get the latest tick.
             */
            Console.WriteLine("Cold Observable");

            var source = Observable.Interval(TimeSpan.FromSeconds(1));

            var subscription1 = source.Subscribe(
                x => Console.WriteLine("Observer 1: OnNext: {0}", x),
                ex => Console.WriteLine("Observer 1: OnError: {0}", ex.Message),
                () => Console.WriteLine("Observer 1: OnCompleted"));

            var subscription2 = source.Subscribe(
                x => Console.WriteLine("Observer 2: OnNext: {0}", x),
                ex => Console.WriteLine("Observer 2: OnError: {0}", ex.Message),
                () => Console.WriteLine("Observer 2: OnCompleted"));

            Console.WriteLine("Press any key to unsubscribe");
            Console.ReadLine();

            subscription1.Dispose();
            subscription2.Dispose();
        }

        public void HotObservable()
        {
            /* Convert the previous cold observable to a hot observable through the use of the Publish operator.
             * Publish operator, which returns an IConnectableObservable instance we name hot.
             * The Publish operator provides a mechanism to share subscriptions by broadcasting a single subscription
             * to multiple subscribers. hot acts as a proxy and subscribes to source, then as it receives values from
             * source, pushes them to its own subscribers. To establish a subscription to the backing source and start
             * receiving values, we use the IConnectableObservable.Connect() method. Since IConnectableObservable
             * inherits IObservable, we can use Subscribe to subscribe to this hot sequence even before it starts running.
             * Notice that in the example, the hot sequence has not been started when subscription1 subscribes to it.
             * Therefore, no value is pushed to the subscriber. After calling Connect, values are then pushed to subscription1.
             * After a delay of 3 seconds, subscription2 subscribes to hot and starts receiving the values immediately from
             * the current position (3 in this case) until the end.
             */
            Console.WriteLine("Hot Observable");

            Console.WriteLine($"Current Time: {DateTime.UtcNow}");

            var source = Observable.Interval(TimeSpan.FromSeconds(1));

            IConnectableObservable<long> hot = source.Publish(); // Convert to hot sequence

            var sub1 = hot.Subscribe(
                x => Console.WriteLine("Observer 1: OnNext: {0}", x),
                ex => Console.WriteLine("Observer 1: OnError: {0}", ex.Message),
                () => Console.WriteLine("Observer 1: OnCompleted"));

            Console.WriteLine("Current Time after 1st subscription: " + DateTime.Now);

            Thread.Sleep(3000); // idle for 3 seconds

            hot.Connect(); // hot is connected to source and start pushing values to subscribers

            Console.WriteLine("Current Time after Connect: " + DateTime.Now);

            Thread.Sleep(3000); // idle for 3 seconds

            Console.WriteLine("Current Time just before 2nd subscription: " + DateTime.Now);

            var sub2 = hot.Subscribe( // value will immediately be pushed to second subscription
                x => Console.WriteLine("Observer 2: OnNext: {0}", x),
                ex => Console.WriteLine("Observer 2: OnError: {0}", ex.Message),
                () => Console.WriteLine("Observer 2: OnCompleted"));

            Console.ReadLine();
            sub1.Dispose();
            sub2.Dispose();
        }
    }
}