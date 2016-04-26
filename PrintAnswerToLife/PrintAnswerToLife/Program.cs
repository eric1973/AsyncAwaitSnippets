using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrintAnswerToLife
{
    class Program
    {
        static void Main(string[] args)
        {

            var printTask = PrintAnswerToLife();
            //printTask.Wait();

            while (!printTask.IsCompleted)
            {
                Log($"Main thread: Do some main work..... in thread:{Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(2000);
            }

            Log($"Main thread: PrintAnswerToLife finished in thread:{Thread.CurrentThread.ManagedThreadId}");

            // The same as above without the use of the compiler service of the 'async' and 'await' syntax sugar.
            var printTaskVerbose = PrintAnswerToLifeVerbose();
            //printTaskVerbose.Wait();

            while (!printTaskVerbose.IsCompleted)
            {
                Log($"Main thread: Do some main work..... in thread:{Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(2000);
            }

            Log($"Main thread: PrintAnswerToLifeVerbose finished in thread:{Thread.CurrentThread.ManagedThreadId}");

            Console.ReadLine();
        }

        private static async Task PrintAnswerToLife()
        {
            await Task.Delay(5000);
            Log($"Task thread: PrintAnswerToLife continuation in thread:{Thread.CurrentThread.ManagedThreadId}");
            ContinueHere();
        }

        private static Task PrintAnswerToLifeVerbose()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            TaskAwaiter awaiter = Task.Delay(5000).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                Log($"Task thread: PrintAnswerToLifeVerbose continuation in thread:{Thread.CurrentThread.ManagedThreadId}");
                try
                {
                    // Ends the awaited task
                    awaiter.GetResult(); // Re-throw any eoccured exception

                    ContinueHere();

                    tcs.SetResult(null); // Return type is 'Task'. So nothing to set as result.
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }

        private static void ContinueHere()
        {
            int answer = 42;
            Log($"Task thread: Value:{answer} , Execute 'ContinueHere' inside continuation on thread:{Thread.CurrentThread.ManagedThreadId}");
        }

        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
