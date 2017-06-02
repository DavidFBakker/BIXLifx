using System;
using Microsoft.VisualBasic.CompilerServices;
using BIXLIFX;
using Microsoft.Extensions.Configuration;
using Util;

namespace LifxConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine($"Running on {Config.OS()}");

            BixLifx.Init();

            Console.ReadLine();

        }
    }
}