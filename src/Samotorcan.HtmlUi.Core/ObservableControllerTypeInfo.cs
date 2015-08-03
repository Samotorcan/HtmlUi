using System.Collections.Generic;

namespace Samotorcan.HtmlUi.Core
{
    internal class ObservableControllerTypeInfo
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
        public Dictionary<string, ControllerProperty> Properties { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableControllerTypeInfo"/> class.
        /// </summary>
        public ObservableControllerTypeInfo()
        {
            Properties = new Dictionary<string, ControllerProperty>();
        }

        #endregion
    }
}
