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
    /// Synchronize max depth exception.
    /// </summary>
    [Serializable]
    public class SyncMaxDepthException : Exception, INativeException
    {
        #region Properties
        #region Public

        #region SyncMaxDepth
        /// <summary>
        /// Gets the synchronize maximum depth.
        /// </summary>
        /// <value>
        /// The synchronize maximum depth.
        /// </value>
        public int SyncMaxDepth { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncMaxDepthException"/> class.
        /// </summary>
        public SyncMaxDepthException()
            : base("Synchronize max depth reached.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncMaxDepthException"/> class.
        /// </summary>
        /// <param name="syncMaxDepth">The synchronize maximum depth.</param>
        public SyncMaxDepthException(int syncMaxDepth)
            : base("Synchronize max depth reached.")
        {
            SyncMaxDepth = syncMaxDepth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncMaxDepthException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="syncMaxDepth">The synchronize maximum depth.</param>
        public SyncMaxDepthException(string message, int syncMaxDepth)
            : base(message)
        {
            SyncMaxDepth = syncMaxDepth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncMaxDepthException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public SyncMaxDepthException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncMaxDepthException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public SyncMaxDepthException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncMaxDepthException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public SyncMaxDepthException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncMaxDepthException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">info</exception>
        protected SyncMaxDepthException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            SyncMaxDepth = info.GetInt32("SyncMaxDepth");
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

            info.AddValue("SyncMaxDepth", SyncMaxDepth);
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
                Type = "SyncMaxDepthException",
                InnerException = InnerException != null ? ExceptionUtility.CreateJavascriptException(InnerException) : null,
                AdditionalData = new Dictionary<string, object>
                {
                    { "SyncMaxDepth", SyncMaxDepth }
                }
            };
        }
        #endregion

        #endregion
        #endregion
    }
}
