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
    /// Parameter mismatch exception.
    /// </summary>
    [Serializable]
    public class ParameterMismatchException : Exception, INativeException
    {
        #region Properties
        #region Public

        #region Parameter
        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public int Parameter { get; private set; }
        #endregion
        #region ExpectedType
        /// <summary>
        /// Gets the expected type.
        /// </summary>
        /// <value>
        /// The expected type.
        /// </value>
        public string ExpectedType { get; private set; }
        #endregion
        #region GotType
        /// <summary>
        /// Gets the type of the got.
        /// </summary>
        /// <value>
        /// The type of the got.
        /// </value>
        public string GotType { get; private set; }
        #endregion
        #region MethodName
        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        public string MethodName { get; private set; }
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
        /// Initializes a new instance of the <see cref="ParameterMismatchException"/> class.
        /// </summary>
        public ParameterMismatchException()
            : base("Parameter mismatch.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMismatchException"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="gotType">Type of the got.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="controllerName">Name of the controller.</param>
        public ParameterMismatchException(int parameter, string expectedType, string gotType, string methodName, string controllerName)
            : base("Parameter mismatch.")
        {
            Parameter = parameter;
            ExpectedType = expectedType;
            GotType = gotType;
            MethodName = methodName;
            ControllerName = controllerName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMismatchException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="gotType">Type of the got.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="controllerName">Name of the controller.</param>
        public ParameterMismatchException(string message, int parameter, string expectedType, string gotType, string methodName, string controllerName)
            : base(message)
        {
            Parameter = parameter;
            ExpectedType = expectedType;
            GotType = gotType;
            MethodName = methodName;
            ControllerName = controllerName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMismatchException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public ParameterMismatchException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMismatchException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ParameterMismatchException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMismatchException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public ParameterMismatchException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMismatchException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">info</exception>
        protected ParameterMismatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Parameter = info.GetInt32("Parameter");
            ExpectedType = info.GetString("ExpectedType");
            GotType = info.GetString("GotType");
            MethodName = info.GetString("MethodName");
            ControllerName = info.GetString("ControllerName");
        }

        #endregion
        #region Methods
        #region Public

        #region GetObjectData
        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
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

            info.AddValue("Parameter", Parameter);
            info.AddValue("ExpectedType", ExpectedType);
            info.AddValue("GotType", GotType);
            info.AddValue("MethodName", MethodName);
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
                Type = "ParameterMismatchException",
                InnerException = InnerException != null ? ExceptionUtility.CreateJavascriptException(InnerException) : null,
                AdditionalData = new Dictionary<string, object>
                {
                    { "Parameter", Parameter },
                    { "ExpectedType", ExpectedType },
                    { "GotType", GotType },
                    { "MethodName", MethodName },
                    { "ControllerName", ControllerName }
                }
            };
        }
        #endregion

        #endregion
        #endregion
    }
}
