using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitSnippets
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            // Fire and Forget the 'async' method ComputeSomeAsync just for fun
            // Note: Any exception that occures inside method 'ComputeSomeAsync' can only be caught in the 'UnhandledException' hander of the current AppDomain.

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var computeTask = ComputeSomethingAsync(async (number) =>
            {
                int doubled = number * 2;
                await Task.Delay(500 * doubled); // simulate external I/O operation

                //continue here on 'Delay' Task completion
                Log($"ComputeSomeAsync: In continuation of calculateFunc for number:{doubled} in thread:{Thread.CurrentThread.ManagedThreadId}");
                return doubled;
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Log($"Main Thread: Waiting for ComputeSomeAsync to complete in thread:{Thread.CurrentThread.ManagedThreadId}");

            while (!computeTask.IsCompleted)
            {
                Log($"Main Thread: Do some main work... in thread:{Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(2000);
            }

            Log($"Main Thread: Work finished");
            Console.ReadLine();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log(e.ExceptionObject.ToString());
        }

        private static async Task ComputeSomethingAsync(Func<int, Task<int>> calculateFunc)
        {
            for (int i = 0; i < 7; i++)
            {
                int result = await calculateFunc(i);
                Log($"ComputeSomeAsync: In continuation of ComputeSomeAsync for result:{ result} in thread:{Thread.CurrentThread.ManagedThreadId}");
            }

            Log("ComputeSomeAsync: Work finished");
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }

    }
}
