using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PrintAnswerToLife
{
    class Program
    {
        static void Main(string[] args)
        {

            var printTask = PrintAnswerToLife();

            printTask.Wait();
            Log("main thread: PrintAnswerToLife finished");

            // The same as above without the use of the compiler service of the 'async' and 'await' syntax sugar.
            var printTaskVerbose = PrintAnswerToLifeVerbose();
            printTaskVerbose.Wait();

            Log("main thread: PrintAnswerToLifeVerbose finished");

            Console.ReadLine();
        }

        private static async Task PrintAnswerToLife()
        {
            await Task.Delay(5000);
            ContinueHere();
        }

        private static Task PrintAnswerToLifeVerbose()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            TaskAwaiter awaiter = Task.Delay(20000).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
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
            int answer = 21 * 2;
            Log(answer.ToString());
        }

        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
