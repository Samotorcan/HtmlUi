using Newtonsoft.Json.Linq;
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
        public Dictionary<string, JToken> Properties { get; set; }
        #endregion
        #region ObservableCollections
        /// <summary>
        /// Gets or sets the observable collections.
        /// </summary>
        /// <value>
        /// The observable collections.
        /// </value>
        public Dictionary<string, ObservableCollectionChanges> ObservableCollections { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerChange"/> class.
        /// </summary>
        public ControllerChange()
        {
            Properties = new Dictionary<string, JToken>();
            ObservableCollections = new Dictionary<string, ObservableCollectionChanges>();
        }

        #endregion
    }
}
