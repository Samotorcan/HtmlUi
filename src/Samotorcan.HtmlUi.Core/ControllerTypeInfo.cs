using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller type info.
    /// </summary>
    internal class ControllerTypeInfo
    {
        #region Properties
        #region Public

        #region Properties
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public List<ControllerProperty> Properties { get; set; }
        #endregion
        #region Methods
        /// <summary>
        /// Gets or sets the methods.
        /// </summary>
        /// <value>
        /// The methods.
        /// </value>
        public List<ControllerMethod> Methods { get; set; }
        #endregion
        #region InternalMethods
        /// <summary>
        /// Gets or sets the internal methods.
        /// </summary>
        /// <value>
        /// The internal methods.
        /// </value>
        public List<ControllerMethod> InternalMethods { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerTypeInfo"/> class.
        /// </summary>
        public ControllerTypeInfo()
        {
            Properties = new List<ControllerProperty>();
            Methods = new List<ControllerMethod>();
            InternalMethods = new List<ControllerMethod>();
        }

        #endregion
    }
}
