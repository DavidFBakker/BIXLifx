using System;
using System.Net.Http;
using System.Threading;

using Util;

namespace LoadTester
{
    class Program
    {

        private static string BaseUrl = @"http://192.168.85.12:9105/?";
        private static Random _random;
        private static HttpClient Client = new HttpClient();
        static void Main(string[] args)
        {
          //  BaseUrl = @"http://127.0.0.1:9105/?";
            var bulb = "Office";
            var speed = .4 * 1024;
            _random = new Random();
            var errors = 0;
            var runs = 0;

            var colors = BIXColors.AvailColors;

            while (true)
            {
                var d = _random.Next(20, 100);
                var r = _random.Next(2, colors.Count);
                var colorRandom = colors[r];

                var power = "On";
                if (r % 3 == 0)
                    power = "Off";

                var cmd = $"Light={bulb}&Power={power}&Color={colorRandom}&Dim={d}";
              
                Log.Bulb($"{bulb} {power} {d} {colorRandom} {runs++}/{errors}");
                var page = $"{BaseUrl}{cmd}";
                
                try
                {
                    var result=Client.GetAsync(page).Result;
                  
                }
                catch (Exception ex)
                {
                  
                    errors++;
                }
                Thread.Sleep((int)(speed));
            }
        }
    }
}
