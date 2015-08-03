using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Samotorcan.HtmlUi.Core.Exceptions
{
    /// <summary>
    /// Content not found exception.
    /// </summary>
    [Serializable]
    public class ContentNotFoundException : Exception
    {
        #region Properties
        #region Public

        #region Path
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNotFoundException"/> class.
        /// </summary>
        public ContentNotFoundException()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNotFoundException"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public ContentNotFoundException(string path)
            : base("Content not found.")
        {
            Path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="path">The path.</param>
        public ContentNotFoundException(string message, string path)
            : base(message)
        {
            Path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNotFoundException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public ContentNotFoundException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ContentNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNotFoundException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public ContentNotFoundException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }


        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">info</exception>
        protected ContentNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Path = info.GetString("Path");
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

            info.AddValue("Path", Path);
        }
        #endregion

        #endregion
        #endregion
    }
}
