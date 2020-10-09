using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleTests
{
    static class ThreadPoolTests
    {
        public static void Start()
        {
            var messages = Enumerable.Range(1, 500)
                .Select(i => $"Messages {i}")
                .ToArray();

            var start = DateTime.Now;
            var timer = Stopwatch.StartNew();

            ThreadPool.GetAvailableThreads(out var available_worker_threads, out var available_complection_threads);
            ThreadPool.GetMinThreads(out var min_worker_threads, out var min_complection_threads);
            ThreadPool.GetMaxThreads(out var max_worker_threads, out var max_complection_threads);

            //ThreadPool.SetMinThreads(4, 4);
            //ThreadPool.SetMaxThreads(16, 16);

            for (var i = 0; i < messages.Length; i++)
            {
                //var local_i = i;
                //new Thread(() => ProcessMessages(messages[local_i])) { IsBackground = true }.Start();
                ThreadPool.QueueUserWorkItem(o => ProcessMessages((string)o), messages[i]);
            }

            var delta = DateTime.Now - start;

            timer.Stop();
            Console.Title = "Обработка заняла" + timer.Elapsed.TotalSeconds;
        }

        private static void ProcessMessages(string message)
        {
            Console.WriteLine("Обработка сообщения {0}", message);
            Thread.Sleep(5000);
            Console.WriteLine("Обработка сообщения {0} закончена", message);
        }
    }
}
