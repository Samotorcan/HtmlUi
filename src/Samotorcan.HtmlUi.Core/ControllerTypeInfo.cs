using System.Collections.Generic;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller type info.
    /// </summary>
    internal class ControllerTypeInfo
    {
        #region Properties
        #region Public

        #region Methods
        /// <summary>
        /// Gets or sets the methods.
        /// </summary>
        /// <value>
        /// The methods.
        /// </value>
        public Dictionary<string, ControllerMethod> Methods { get; set; }
        #endregion
        #region InternalMethods
        /// <summary>
        /// Gets or sets the internal methods.
        /// </summary>
        /// <value>
        /// The internal methods.
        /// </value>
        public Dictionary<string, ControllerMethod> InternalMethods { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerTypeInfo"/> class.
        /// </summary>
        public ControllerTypeInfo()
        {
            Methods = new Dictionary<string, ControllerMethod>();
            InternalMethods = new Dictionary<string, ControllerMethod>();
        }

        #endregion
    }
}
