using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller property.
    /// </summary>
    internal class ControllerProperty : ControllerPropertyBase
    {
        #region Properties
        #region Public

        #region Access
        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public Access Access { get; set; }
        #endregion
        #region PropertyType
        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public Type PropertyType { get; set; }
        #endregion
        #region GetDelegate
        /// <summary>
        /// Gets or sets the get delegate.
        /// </summary>
        /// <value>
        /// The get delegate.
        /// </value>
        public Delegate GetDelegate { get; set; }
        #endregion
        #region SetDelegate
        /// <summary>
        /// Gets or sets the set delegate.
        /// </summary>
        /// <value>
        /// The set delegate.
        /// </value>
        public Delegate SetDelegate { get; set; }
        #endregion

        #endregion
        #endregion
    }
}
