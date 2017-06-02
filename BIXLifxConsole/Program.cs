using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BIXLifxConsole
{
    internal class Program
    {
        private static readonly HttpClient Client = new HttpClient();

        private static int Main(string[] args)
        {
            //  Console.WriteLine(Environment.CommandLine);
            if (!Environment.CommandLine.Contains(" "))
            {
                Console.WriteLine("Enter json formatted command");
                Console.WriteLine("[{\"Label\":\"Office 1\",\"Power\":\"On\",\"Dim\":10,\"Color\":\"Blue\"}]");
                return 1;
            }

            var json = Environment.CommandLine.Substring(Environment.CommandLine.IndexOf(" ")).Trim();
            // Console.WriteLine($"JSON {json}");
            RunTask(json).ConfigureAwait(true);
            return 0;
        }

        private static async Task RunTask(string arg)
        {
            var page = $"{Config.BaseUrl}CMDS={arg}";
           // Console.WriteLine($"Running: {page}");
            var response = Client.GetAsync(page).Result;
          //  var res = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            await response.Content.ReadAsStringAsync().ConfigureAwait(false);
           // Console.WriteLine(res);
        }
    }
}