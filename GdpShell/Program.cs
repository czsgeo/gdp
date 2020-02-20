using Gdp;
using System;

namespace Gdp
{
    class Program
    {
        static void Main(string[] args)
        {

            LocalGnssExecutor.AppFolder = "./";
            LocalGnssExecutor.Execute(args);

            Console.WriteLine("See you!");

        }
    }
}
