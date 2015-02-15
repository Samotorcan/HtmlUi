using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Logs
{
    /// <summary>
    /// General log.
    /// </summary>
    public static class GeneralLog
    {
        #region Properties
        #region Private

        #region Log
        private static ILog _log;
        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        private static ILog Log
        {
            get
            {
                if (_log == null)
                    _log = LogManager.GetLogger("GeneralLog");

                return _log;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Methods
        #region Public

        #region Debug
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Debug level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Debug(object message)
        {
            Log.Debug(message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Debug level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Debug(object message, Exception exception)
        {
            Log.Debug(message, exception);
        }
        #endregion
        #region Warn
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Warn level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Warn(object message)
        {
            Log.Warn(message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Warn level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Warn(object message, Exception exception)
        {
            Log.Warn(message, exception);
        }
        #endregion
        #region Error
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Error level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Error(object message)
        {
            Log.Error(message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Error level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Error(object message, Exception exception)
        {
            Log.Error(message, exception);
        }
        #endregion

        #endregion
        #endregion
    }
}
