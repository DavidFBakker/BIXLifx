using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;

namespace BIXLifxService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication(true) { Name = "Listener" };
            app.HelpOption("-?|-h|--help");

            var port = 9105;

            app.Command("port", config =>
            {
                config.Description = "Port to listen on";
                var portArg = config.Argument("[port number]", "The port number to listen on 2");

                config.OnExecute(() =>
                {
                    port = int.Parse(portArg.Value);
                    return 0;
                });

                config.Command("help", config2 =>
                {
                    config2.Description = "The port number to listen on";
                    config2.OnExecute(() =>
                    {
                        config.ShowHelp("config");
                        return 1;
                    });
                });
            });

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{port};")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
