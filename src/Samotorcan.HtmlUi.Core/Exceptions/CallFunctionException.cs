using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Exceptions
{
    /// <summary>
    /// Call function exception.
    /// </summary>
    [Serializable]
    public class CallFunctionException : Exception
    {
        #region Properties
        #region Public

        #region JavaScriptException
        /// <summary>
        /// Gets the javascript exception.
        /// </summary>
        /// <value>
        /// The javascript exception.
        /// </value>
        public JToken JavaScriptException { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CallFunctionException"/> class.
        /// </summary>
        public CallFunctionException()
            : base("Call function exception.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallFunctionException"/> class.
        /// </summary>
        /// <param name="javaScriptException">The javascript exception.</param>
        public CallFunctionException(JToken javaScriptException)
            : base("Call function exception.")
        {
            JavaScriptException = javaScriptException;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallFunctionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="javaScriptException">The javascript exception.</param>
        public CallFunctionException(string message, JToken javaScriptException)
            : base(message)
        {
            JavaScriptException = javaScriptException;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallFunctionException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public CallFunctionException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallFunctionException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CallFunctionException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallFunctionException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public CallFunctionException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">info</exception>
        protected CallFunctionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            JavaScriptException = (JToken)info.GetValue("JavascriptException", typeof(JToken));
        }

        #endregion
        #region Methods
        #region Public

        #region GetObjectData
        /// <summary>
        /// Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">info</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("JavascriptException", JavaScriptException);
        }
        #endregion

        #endregion
        #endregion
    }
}
