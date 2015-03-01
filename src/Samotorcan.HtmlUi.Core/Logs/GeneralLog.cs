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

        #region Logger
        private static ILog _logger;
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        private static ILog Logger
        {
            get
            {
                if (_logger == null)
                    _logger = LogManager.GetLogger("GeneralLog");

                return _logger;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Methods
        #region Public

        #region Log
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="message">The message.</param>
        public static void Log(LogMessageType messageType, string message)
        {
            if (messageType == LogMessageType.Debug)
                GeneralLog.Debug(message);

            if (messageType == LogMessageType.Info)
                GeneralLog.Info(message);

            if (messageType == LogMessageType.Warn)
                GeneralLog.Warn(message);

            if (messageType == LogMessageType.Error)
                GeneralLog.Error(message);
        }
        #endregion

        #region Debug
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Debug level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Debug(object message)
        {
            Logger.Debug(message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Debug level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Debug(object message, Exception exception)
        {
            Logger.Debug(message, exception);
        }
        #endregion
        #region Info
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Info level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Info(object message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Info level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Info(object message, Exception exception)
        {
            Logger.Info(message, exception);
        }
        #endregion
        #region Warn
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Warn level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Warn(object message)
        {
            Logger.Warn(message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Warn level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Warn(object message, Exception exception)
        {
            Logger.Warn(message, exception);
        }
        #endregion
        #region Error
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Error level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Error(object message)
        {
            Logger.Error(message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Error level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Error(object message, Exception exception)
        {
            Logger.Error(message, exception);
        }
        #endregion

        #endregion
        #endregion
    }
}
