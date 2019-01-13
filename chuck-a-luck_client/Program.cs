using System;

namespace chuck_a_luck_client
{
    class Program
    {
        static void Main(string[] args)
        {
            new SynchronousSocketClient().StartClient();
        }
    }
}
