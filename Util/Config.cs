using System;
using System.Collections.Generic;

using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Util
{
    public class Config
    {

        public static string BasePath
        {
            get
            {
                var basePath = Directory.GetCurrentDirectory();

                if (OS().Equals("Linux"))
                {
                    basePath = @"/media/dotnet/BIXLifxService";
                }
                return basePath;

            }
        }

        public static int GetAppSetting(string key, int defaultValue = 0)
        {
            if (!key.Contains(":"))
            {
                key = "BIXLifx:" + key;
            }

            
            var builder = new ConfigurationBuilder()
                .SetBasePath(BasePath)
                .AddJsonFile("appsettings.json",true);

            var configuration = builder.Build();

            var result = configuration[key] ?? defaultValue.ToString();

            var ret = 0;
            if (string.IsNullOrEmpty(result) || !int.TryParse(result, out ret))
                return defaultValue;

            return ret;
        }

        public static string OS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Windows";

            return "Who Knows";
        }


    }
}
