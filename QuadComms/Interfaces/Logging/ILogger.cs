using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Logging
{
    public interface ILogger
    {
        /// <summary>
        ///  very detailed logs, which may include high-volume information such as protocol payloads. 
        ///  This log level is typically only enabled during development
        /// </summary>
        /// <param name="message">message</param>
        void Trace(string message);

        /// <summary>
        /// debugging information, less detailed than trace, typically not enabled in production environment.
        /// </summary>
        /// <param name="message">message</param>
        void Debug(string message);

        void Info(string message);

        void Error(string message);

    }
}
