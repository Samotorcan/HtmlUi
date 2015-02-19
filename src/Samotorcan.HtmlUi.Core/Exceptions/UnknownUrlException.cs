using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Exceptions
{
    /// <summary>
    /// Unknown url exception.
    /// </summary>
    [Serializable]
    public class UnknownUrlException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownUrlException"/> class.
        /// </summary>
        public UnknownUrlException()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownUrlException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnknownUrlException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownUrlException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public UnknownUrlException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownUrlException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public UnknownUrlException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownUrlException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public UnknownUrlException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownUrlException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected UnknownUrlException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
