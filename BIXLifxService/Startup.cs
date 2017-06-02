using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Util;
using BIXLIFX;

namespace BIXLifxService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Warning);

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();


            BixLifx.Init(false);

            app.Run(async context =>
            {
                if (context.Request.QueryString.HasValue)
                {
                    var eventID = Guid.NewGuid().ToString();

                    Log.Bulb($"{eventID} Received new event");

                    var qs = new Dictionary<string, string>();

                    var queryDictionary = QueryHelpers.ParseQuery(context.Request.QueryString.Value);

                    foreach (var command in queryDictionary)
                        qs[command.Key.ToLower()] = command.Value.FirstOrDefault();


                    Util.Log.Info($"{eventID} Handling event");
                    var resp = await BixLifx.DoCommands(eventID,  qs);


                    await context.Response.WriteAsync(resp);
                }
            });
        }
    }
}