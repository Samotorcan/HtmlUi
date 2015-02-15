using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Exceptions
{
    /// <summary>
    /// Cef process exception.
    /// </summary>
    [Serializable]
    public class CefProcessException : Exception
    {
        #region Properties
        #region Public

        #region ExitCode
        /// <summary>
        /// Gets or sets the exit code.
        /// </summary>
        /// <value>
        /// The exit code.
        /// </value>
        public int ExitCode { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CefProcessException"/> class.
        /// </summary>
        public CefProcessException()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CefProcessException"/> class.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        public CefProcessException(int exitCode)
            : base()
        {
            ExitCode = exitCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CefProcessException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CefProcessException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CefProcessException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public CefProcessException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CefProcessException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CefProcessException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CefProcessException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public CefProcessException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CefProcessException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected CefProcessException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
