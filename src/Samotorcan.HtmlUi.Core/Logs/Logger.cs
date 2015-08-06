using log4net;
using log4net.Appender;
using System;

namespace Samotorcan.HtmlUi.Core.Logs
{
    /// <summary>
    /// Logger.
    /// </summary>
    public static class Logger
    {
        #region Properties
        #region Public

        #region LogSeverity
        /// <summary>
        /// Gets or sets the log severity.
        /// </summary>
        /// <value>
        /// The log severity.
        /// </value>
        public static LogSeverity LogSeverity { get; set; }
        #endregion

        #endregion
        #region Private

        #region InternalLogger
        private static ILog _internalLogger;
        /// <summary>
        /// Gets the internal logger.
        /// </summary>
        /// <value>
        /// The internal logger.
        /// </value>
        private static ILog InternalLogger
        {
            get
            {
                if (_internalLogger == null)
                    _internalLogger = LogManager.GetLogger("GeneralLog");

                return _internalLogger;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Methods
        #region Public

        #region Log
        /// <summary>
        /// Logs the specified message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Log(LogMessageType messageType, object message, Exception exception)
        {
            if (messageType == LogMessageType.Debug)
            {
                if (LogSeverity == LogSeverity.Debug)
                {
                    InternalLogger.Debug(message, exception);
                }
            }
            else if (messageType == LogMessageType.Info)
            {
                if (LogSeverity == LogSeverity.Debug || LogSeverity == LogSeverity.Info)
                {
                    InternalLogger.Info(message, exception);
                }
            }
            else if (messageType == LogMessageType.Warn)
            {
                if (LogSeverity == LogSeverity.Debug || LogSeverity == LogSeverity.Info || LogSeverity == LogSeverity.Warn)
                {
                    InternalLogger.Warn(message, exception);
                }
            }
            else if (messageType == LogMessageType.Error)
            {
                if (LogSeverity == LogSeverity.Debug || LogSeverity == LogSeverity.Info || LogSeverity == LogSeverity.Warn || LogSeverity == LogSeverity.Error)
                {
                    InternalLogger.Error(message, exception);
                }
            }
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="message">The message.</param>
        public static void Log(LogMessageType messageType, object message)
        {
            Log(messageType, message, null);
        }
        #endregion

        #region Debug
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Debug level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Debug(object message)
        {
            Log(LogMessageType.Debug, message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Debug level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Debug(object message, Exception exception)
        {
            Log(LogMessageType.Debug, message, exception);
        }
        #endregion
        #region Info
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Info level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Info(object message)
        {
            Log(LogMessageType.Info, message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Info level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Info(object message, Exception exception)
        {
            Log(LogMessageType.Info, message, exception);
        }
        #endregion
        #region Warn
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Warn level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Warn(object message)
        {
            Log(LogMessageType.Warn, message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Warn level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Warn(object message, Exception exception)
        {
            Log(LogMessageType.Warn, message, exception);
        }
        #endregion
        #region Error
        /// <summary>
        /// Log a message object with the log4net.Core.Level.Error level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        public static void Error(object message)
        {
            Log(LogMessageType.Error, message);
        }

        /// <summary>
        /// Log a message object with the log4net.Core.Level.Error level including the stack trace of the System.Exception passed as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public static void Error(object message, Exception exception)
        {
            Log(LogMessageType.Error, message, exception);
        }
        #endregion

        #region Flush
        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public static void Flush()
        {
            var rep = LogManager.GetRepository();

            foreach (var appender in rep.GetAppenders())
            {
                var buffered = appender as BufferingAppenderSkeleton;
                if (buffered != null)
                    buffered.Flush();
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
