using Newtonsoft.Json;
using System.Collections.Generic;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// ObservableCollectionChanges
    /// </summary>
    internal class ObservableCollectionChanges
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
        #region Actions
        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        public List<ObservableCollectionChange> Actions { get; set; }
        #endregion
        #region IsReset
        /// <summary>
        /// Gets or sets a value indicating whether this instance is reset.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is reset; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsReset { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionChanges"/> class.
        /// </summary>
        public ObservableCollectionChanges()
        {
            Actions = new List<ObservableCollectionChange>();
        }

        #endregion
    }
}
