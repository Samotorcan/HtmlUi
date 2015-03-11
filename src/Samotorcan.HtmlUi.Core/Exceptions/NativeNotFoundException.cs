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
    /// Native not found exception.
    /// </summary>
    [Serializable]
    public class NativeNotFoundException : Exception, INativeException
    {
        #region Properties
        #region Public

        #region NativeName
        /// <summary>
        /// Gets the name of the native.
        /// </summary>
        /// <value>
        /// The name of the native.
        /// </value>
        public string NativeName { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeNotFoundException"/> class.
        /// </summary>
        public NativeNotFoundException()
            : base("Native not found.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeNotFoundException"/> class.
        /// </summary>
        /// <param name="nativeName">Name of the native.</param>
        public NativeNotFoundException(string nativeName)
            : base("Native not found.")
        {
            NativeName = nativeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nativeName">Name of the native.</param>
        public NativeNotFoundException(string message, string nativeName)
            : base(message)
        {
            NativeName = nativeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeNotFoundException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public NativeNotFoundException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public NativeNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeNotFoundException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public NativeNotFoundException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">info</exception>
        protected NativeNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            NativeName = info.GetString("NativeName");
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

            info.AddValue("NativeName", NativeName);
        }
        #endregion

        #endregion
        #region Internal

        #region ToJavascriptException
        /// <summary>
        /// To the javascript exception.
        /// </summary>
        /// <returns></returns>
        JavascriptException INativeException.ToJavascriptException()
        {
            return new JavascriptException
            {
                Message = Message,
                Type = "NativeNotFoundException",
                InnerException = InnerException != null ? ExceptionUtility.CreateJavascriptException(InnerException) : null,
                AdditionalData = new Dictionary<string, object>
                {
                    { "NativeName", NativeName }
                }
            };
        }
        #endregion

        #endregion
        #endregion
    }
}
