using System;
using QuestradeClient;

namespace PortfolioApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var client = new QuestradeClientBloop();
            client.GetStuff().Wait();
            Console.WriteLine("Done");
        }
    }
}
