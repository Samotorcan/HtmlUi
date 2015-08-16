using System;

namespace Samotorcan.HtmlUi.Core.Logs
{
    /// <summary>
    /// Log message type.
    /// </summary>
    public sealed class LogMessageType
    {
        #region LogTypes

        #region Debug
        /// <summary>
        /// The debug.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It's immutable.")]
        public static readonly LogMessageType Debug = new LogMessageType("Debug");
        #endregion
        #region Info
        /// <summary>
        /// The info.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It's immutable.")]
        public static readonly LogMessageType Info = new LogMessageType("Info");
        #endregion
        #region Warn
        /// <summary>
        /// The warn.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It's immutable.")]
        public static readonly LogMessageType Warn = new LogMessageType("Warn");
        #endregion
        #region Error
        /// <summary>
        /// The error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It's immutable.")]
        public static readonly LogMessageType Error = new LogMessageType("Error");
        #endregion

        #endregion
        #region Properties
        #region Public

        #region Value
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private LogMessageType(string value)
        {
            Value = value;
        }

        #endregion
        #region Methods
        #region Public

        #region Parse
        /// <summary>
        /// Parses the specified log message type.
        /// </summary>
        /// <param name="logMessageType">The log message type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">log message type</exception>
        /// <exception cref="System.ArgumentException">Invalid log message type.;logMessageType</exception>
        public static LogMessageType Parse(string logMessageType)
        {
            if (string.IsNullOrWhiteSpace(logMessageType))
                throw new ArgumentNullException("logMessageType");

            if (logMessageType == Debug.Value)
                return Debug;

            if (logMessageType == Info.Value)
                return Info;

            if (logMessageType == Warn.Value)
                return Warn;

            if (logMessageType == Error.Value)
                return Error;

            throw new ArgumentException("Invalid log message type.", "logMessageType");
        }
        #endregion

        #endregion
        #endregion
    }
}
