using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    internal class ObservableControllerDescription : ControllerDescription
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
        public List<ControllerPropertyDescription> Properties { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableControllerDescription"/> class.
        /// </summary>
        public ObservableControllerDescription()
        {
            Properties = new List<ControllerPropertyDescription>();
        }

        #endregion
    }
}
