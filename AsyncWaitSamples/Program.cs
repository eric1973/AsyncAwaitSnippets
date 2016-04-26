using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncWaitSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            // Fire and Forget the 'async' method ComputeSomeAsync just for fun
            // Note: Any exception that occures inside method 'ComputeSomeAsync' can only be caught in the 'UnhandledException' hander of the current AppDomain.

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var computeTask = ComputeSomeAsync(async (number) =>
            {
                int doubled = number * 2;
                await Task.Delay(500 * doubled); // simulate external I/O operation
                System.Diagnostics.Debug.WriteLine($"In continuation of calculateFunc for number:{doubled} in thread:{Thread.CurrentThread.ManagedThreadId}");
                return doubled;
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            System.Diagnostics.Debug.WriteLine($"Main Thread other work finished. Waiting for ComputeSomeAsync to complete in thread:{Thread.CurrentThread.ManagedThreadId}");

            while (!computeTask.IsCompleted)
            {
                System.Diagnostics.Debug.WriteLine("Do some main work.");
                Thread.Sleep(10);
            }
            
            System.Diagnostics.Debug.WriteLine($"Main Thread other work finished");
            Console.ReadLine();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
        }

        private static async Task ComputeSomeAsync(Func<int, Task<int>> calculateFunc)
        {
            for (int i = 0; i < 20; i++)
            {
                int result = await calculateFunc(i);
                System.Diagnostics.Debug.WriteLine($"In continuation of ComputeSomeAsync for result:{ result} in thread:{Thread.CurrentThread.ManagedThreadId}");
            }

            System.Diagnostics.Debug.WriteLine("ComputeSomeAsync finished");
        }

    }
}
