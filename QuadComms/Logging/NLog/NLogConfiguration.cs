namespace QuadComms.Logging.NLog
{
    using global::NLog;
    using global::NLog.Config;
    using global::NLog.Targets;
    using QuadComms.Interfaces.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class NLogConfiguration : ILogConfiguration
    {
        public void Configure()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));
            LogManager.Configuration = config;
        }
    }
}
