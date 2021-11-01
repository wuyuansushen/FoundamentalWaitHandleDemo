using System;
using System.Threading;
using System.Diagnostics;

namespace FoundamentalWaitHandleDemo
{
    public class Program
    {
        private static WaitHandle[] waitHandles = new WaitHandle[] {
            new AutoResetEvent(false),
            new AutoResetEvent(false)
        };

        private static Random random = new Random();
        public static void Main(string[] args)
        {
            using (waitHandles[0])
            using (waitHandles[1])
            {
                Stopwatch stopwatch = new Stopwatch();
                Console.WriteLine($"Main thread is waiting for BOTH tashs to complete.");
                stopwatch.Start();
                ThreadPool.QueueUserWorkItem(DoSomeTask, waitHandles[0]);
                ThreadPool.QueueUserWorkItem(DoSomeTask, waitHandles[1]);
                WaitHandle.WaitAll(waitHandles);
                stopwatch.Stop();
                Console.WriteLine($"BOTH tasks are completed (time elapsed={stopwatch.ElapsedMilliseconds}ms)");
                stopwatch.Reset();

                //AutoResetEvent has reset.
                Console.WriteLine($"Main is waiting for any one task to complete.");
                stopwatch.Start();
                ThreadPool.QueueUserWorkItem(DoSomeTask, waitHandles[0]);
                ThreadPool.QueueUserWorkItem(DoSomeTask, waitHandles[1]);
                WaitHandle.WaitAny(waitHandles);
                stopwatch.Stop();
                Console.WriteLine($"one of tasks are completed (time elapsed={stopwatch.ElapsedMilliseconds}ms)");
            }
        }

        public static void DoSomeTask(object eve)
        {
            AutoResetEvent @event =(AutoResetEvent) eve;
            int timeElapsed = random.Next(3, 10);
            Console.WriteLine($"This work needs {timeElapsed}s.");
            Thread.Sleep(timeElapsed * 1000);
            //Signal the Event.
            @event.Set();
        }
    }
}
