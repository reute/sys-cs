using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FizzBuzz
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i < 101; i++)
            {
                
                if (i % 3 == 0)
                {
                    if (i % 5 == 0)
                    {
                        Console.WriteLine("FizzBuzz");
                        continue;
                    }
                    Console.WriteLine("Fizz");
                    continue;
                }

                if (i % 5 == 0)
                {
                    Console.WriteLine("Buzz");
                    continue;
                }

                Console.WriteLine(i);
            }
            Console.ReadKey();
        }
    }
}
