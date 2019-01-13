using System;

namespace chuck_a_luck_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new SynchronousSocketListener().StartListening();
        }
    }
}
