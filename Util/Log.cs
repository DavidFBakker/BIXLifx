using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Util
{
    public static class Log
    {
        public const int MaxMessagesDef = 100;
        private static ILoggerFactory _Factory;
        private static readonly ILogger Logger = CreateLogger();
        public static int MaxMessages = MaxMessagesDef;
        private static readonly List<string> ValidMessages = new List<string> {"ERROR", "DEBUG", "BULB", "INFO"};

        static Log()
        {
            MaxMessages = Config.GetAppSetting("MaxMessages", MaxMessagesDef);
            Messages = new LinkedList<string>();
          
        }

        public static LinkedList<string> Messages { get; set; }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                    ConfigureLogger(_Factory);
                }
                return _Factory;
            }
            set => _Factory = value;
        }

        public static ILogger CreateLogger()
        {
            return LoggerFactory.CreateLogger("LIFX");
        }

        public static void ConfigureLogger(ILoggerFactory factory)
        {
            //   factory.AddDebug(LogLevel.None);
            factory.AddConsole();
            factory.AddFile(Config.BasePath +"/logFileFromHelper.log"); //serilog file extension           
        }

        public static string GetMessages()
        {
            var builder = new StringBuilder();

            var messages = Messages.ToList();

            builder.AppendLine(
                $"<font color=\"{GetColor("INFO")}\"> <strong>INFO: </strong></font> Max Messages is set to: {MaxMessages}<br>");

            foreach (var message in messages)
            {
                if (message == null || !message.Contains(":"))
                    continue;

                var level = message.Substring(0, message.IndexOf(":"));
                if (!ValidMessages.Contains(level))
                    continue;

                var message1 = message.Remove(0, message.IndexOf(":"));
                var ccolor = GetColor(level);

                builder.AppendLine($"<font color=\"{ccolor}\"> <strong>{level}</strong></font> {message1}<br>");
            }

            return builder.ToString();
        }

        private static string GetColor(string level)
        {
            if (level == "ERROR")
                return BIXColors.Colors["firebrick"].Hex;
            if (level == "DEBUG")
                return BIXColors.Colors["deeppink"].Hex;
            if (level == "BULB")
                return BIXColors.Colors["darkgoldenrod"].Hex;

            return BIXColors.Colors["darkgreen"].Hex;
        }


        private static void AddMessage(string message)
        {
            if (message == null)
                return;

            lock (message)
            {
                MaxMessages = Config.GetAppSetting("MaxMessages", MaxMessagesDef);
                if (Messages.Count >= MaxMessages)
                    Messages.RemoveFirst();

                Messages.AddLast(message);
            }
        }


        public static void Info(string msg)
        {
            msg = $"INFO: {msg}";
            AddMessage(msg);
            Logger.LogInformation(msg);
        }

        public static void Bulb(string msg)
        {
            msg = $"BULB: {msg}";
            AddMessage(msg);
            Logger.LogInformation(msg);
        }

        public static void Error(string msg)
        {
            msg = $"ERROR: {msg}";
            AddMessage(msg);
            Logger.LogError(msg);
        }
    }
}