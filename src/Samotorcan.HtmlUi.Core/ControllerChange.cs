using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller change.
    /// </summary>
    internal class ControllerChange
    {
        #region Properties
        #region Public

        #region Id
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
        #endregion
        #region Properties
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public Dictionary<string, object> Properties { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerChange"/> class.
        /// </summary>
        public ControllerChange()
        {
            Properties = new Dictionary<string, object>();
        }

        #endregion
    }
}
