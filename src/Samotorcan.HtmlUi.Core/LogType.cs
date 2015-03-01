using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Log type.
    /// </summary>
    internal sealed class LogType
    {
        #region LogTypes

        #region GeneralLog
        /// <summary>
        /// The debug.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It's immutable.")]
        public static readonly LogType GeneralLog = new LogType("GeneralLog");
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
        /// Initializes a new instance of the <see cref="LogType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private LogType(string value)
        {
            Value = value;
        }

        #endregion
        #region Methods
        #region Public

        #region Parse
        /// <summary>
        /// Parses the specified log type.
        /// </summary>
        /// <param name="logType">The log type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">logType</exception>
        /// <exception cref="System.ArgumentException">Invalid log type.;logType</exception>
        public static LogType Parse(string logType)
        {
            if (string.IsNullOrWhiteSpace(logType))
                throw new ArgumentNullException("logType");

            if (logType == LogType.GeneralLog.Value)
                return LogType.GeneralLog;

            throw new ArgumentException("Invalid log type.", "logType");
        }
        #endregion

        #endregion
        #endregion
    }
}
