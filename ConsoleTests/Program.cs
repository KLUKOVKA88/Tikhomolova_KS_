using System;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //ThreadTests.Start();
            ThreadPoolTests.Start();

            Console.WriteLine("Главный поток работу закончил");
            Console.ReadLine();
        }
    }
}