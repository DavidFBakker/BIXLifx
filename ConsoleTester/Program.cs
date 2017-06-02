using LIFXDevices;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace ConsoleTester
{
    class Program
    {
        private static List<LightBulb> Bulbs { get; set; }
        private static string _baseUrl = @"http://192.168.85.12:9105/?";
        private static HttpClient Client = new HttpClient();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Bulbs = new List<LightBulb>();
            var cmd = $"Light=Office&Json";
            var page = $"{_baseUrl}{cmd}";
            var response = Client.GetAsync(page).Result;
            var contents =  response.Content.ReadAsStringAsync().Result;
            Bulbs = JsonConvert.DeserializeObject<List<LightBulb>>(contents);
        }
    }
}
