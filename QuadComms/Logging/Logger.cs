namespace QuadComms.Logging
{
    using QuadComms.Interfaces.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class TheLogger : ILogger
    {
        private readonly ILog theLogger = LogProvider.GetCurrentClassLogger();

        public TheLogger(ILogConfiguration logConfiguration)
        {
            logConfiguration.Configure();
        }

        public void Trace(string message)
        {
            Task.Run(() =>
            this.theLogger.Trace(message)
            );
        }

        public void Debug(string message)
        {
            Task.Run(()=>
            this.theLogger.Debug(message)
            );
        }


        public void Info(string message)
        {
            Task.Run(()=>
            this.theLogger.Info(message)
            );
        }


        public void Error(string message)
        {
            Task.Run(() =>
            this.theLogger.Error(message)
            );
        }
    }
}
