using System;
using System.Threading.Tasks;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //TPLOverview.Start();
            var task = AsyncAwayTest.StartAsync();
            var process_messages_task = AsyncAwayTest.ProcessDataTestAsync();

            Console.WriteLine("Тестовая задача заупщена и мы ее ждем!...");

            //task.Wait();
            Task.WaitAll(task, process_messages_task);

            Console.WriteLine("Главный поток работу закончил!");
            Console.ReadLine();
        }
    }           
}