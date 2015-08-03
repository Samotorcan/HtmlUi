using System;
using System.Collections.Generic;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller method.
    /// </summary>
    internal class ControllerMethod : ControllerMethodBase
    {
        #region Properties
        #region Public

        #region Delegate
        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public Delegate Delegate { get; set; }
        #endregion
        #region ParameterTypes
        /// <summary>
        /// Gets or sets the parameter types.
        /// </summary>
        /// <value>
        /// The parameter types.
        /// </value>
        public List<Type> ParameterTypes { get; set; }
        #endregion
        #region MethodType
        /// <summary>
        /// Gets or sets the type of the method.
        /// </summary>
        /// <value>
        /// The type of the method.
        /// </value>
        public MethodType MethodType { get; set; }
        #endregion

        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerMethod"/> class.
        /// </summary>
        public ControllerMethod()
        {
            ParameterTypes = new List<Type>();
        }
        #endregion
    }
}
