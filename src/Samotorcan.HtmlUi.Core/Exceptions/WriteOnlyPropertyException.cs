using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Samotorcan.HtmlUi.Core.Exceptions
{
    /// <summary>
    /// Write only property exception.
    /// </summary>
    [Serializable]
    public class WriteOnlyPropertyException : Exception, INativeException
    {
        #region Properties
        #region Public

        #region PropertyName
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; private set; }
        #endregion
        #region ControllerName
        /// <summary>
        /// Gets or sets the name of the controller.
        /// </summary>
        /// <value>
        /// The name of the controller.
        /// </value>
        public string ControllerName { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyPropertyException"/> class.
        /// </summary>
        public WriteOnlyPropertyException()
            : base("Write only property.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="controllerName">Name of the controller.</param>
        public WriteOnlyPropertyException(string propertyName, string controllerName)
            : base("Write only property.")
        {
            PropertyName = propertyName;
            ControllerName = controllerName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="controllerName">Name of the controller.</param>
        public WriteOnlyPropertyException(string message, string propertyName, string controllerName)
            : base(message)
        {
            PropertyName = propertyName;
            ControllerName = controllerName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public WriteOnlyPropertyException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public WriteOnlyPropertyException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public WriteOnlyPropertyException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">info</exception>
        protected WriteOnlyPropertyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            PropertyName = info.GetString("PropertyName");
            ControllerName = info.GetString("ControllerName");
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

            info.AddValue("PropertyName", PropertyName);
            info.AddValue("ControllerName", ControllerName);
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
                Type = "WriteOnlyPropertyException",
                InnerException = InnerException != null ? ExceptionUtility.CreateJavascriptException(InnerException) : null,
                AdditionalData = new Dictionary<string, object>
                {
                    { "ControllerName", ControllerName },
                    { "PropertyName", PropertyName }
                }
            };
        }
        #endregion

        #endregion
        #endregion
    }
}
