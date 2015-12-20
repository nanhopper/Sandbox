using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox
{
    class Program
    {
        private delegate void NotifyPropertyChangedEventHandler(object sender, NotifyEventArgs args);

        private static event NotifyPropertyChangedEventHandler NotifyPropertyChanged;

        static void Main(string[] args)
        {            
            NotifyPropertyChanged += OnNotifyPropertyChanged;

            while (true)
            {
                var read = Console.ReadKey(true);
                if(read.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("Enter key pressed, exiting loop. Press Enter key again to stop program.");
                    break;
                }
                WriteLine("Key pressed: " + read.Key);
                DoLongTask1();
            }
            Console.ReadLine();
        }

        private static void OnNotifyPropertyChanged(object sender, NotifyEventArgs args)
        {
            WriteLine("Result received: " + args.Result);
        }

        private static void DoLongTask0()
        {
            var results = DoWork();
            WriteResult(results);
        }

        private async static Task DoLongTask1()
        {
            var results = await Task.Run(() => DoWork().ToList());
            WriteResult(results);
        }        

        private async static Task DoLongTask2()
        {
            var task = new Task<IEnumerable<int>>(() => DoWork());
            task.Start();
            var result = await task;
            WriteResult(result);
        }

        private async static Task DoLongTask3()
        {
            var results = await Task.Factory.StartNew(()=> DoWork());
            WriteResult(results);
        }

        private static IEnumerable<int> DoWork()
        {
            WriteLine("Starting long task...");
            foreach(var result in GetResults())
            {
                yield return result;
            }
            WriteLine("Task done.");
        }

        private static void WriteResult(int result)
        {
            WriteLine("Long task done. Result is: " + result);
        }

        private static void WriteResult(IEnumerable<int> results)
        {
            foreach (var result in results)
            {
                NotifyPropertyChanged(null, new NotifyEventArgs { Result = result.ToString() });
                //WriteLine("Intermediary result is: " + result);
            }
        }

        private static void WriteLine(string message)
        {
            Console.WriteLine(string.Format("{0} [{1}]", message, Thread.CurrentThread.ManagedThreadId));
        }

        private static IEnumerable<int> GetResults()
        {
            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(3000);
                yield return random.Next();
            }
        }

        private static int GetResult()
        {
            return random.Next();
        }

        private static Random random = new Random(DateTime.Now.Millisecond);

        private class NotifyEventArgs
        {
            public string Result { get; set; }
        }
    }
}
