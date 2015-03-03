using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller description.
    /// </summary>
    internal class ControllerDescription
    {
        #region Properties
        #region Public

        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        #endregion
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

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerDescription"/> class.
        /// </summary>
        public ControllerDescription()
        {
            Properties = new List<ControllerProperty>();
            Methods = new List<ControllerMethod>();
        }

        #endregion
    }
}
