using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        #region IsObservableCollection
        /// <summary>
        /// Gets or sets a value indicating whether this instance is observable collection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is observable collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsObservableCollection { get; set; }
        #endregion
        #region ObservableCollection
        /// <summary>
        /// Gets or sets the observable collection.
        /// </summary>
        /// <value>
        /// The observable collection.
        /// </value>
        public INotifyCollectionChanged ObservableCollection { get; set; }
        #endregion
        #region ObservableCollectionItems
        /// <summary>
        /// Gets or sets the observable collection items.
        /// </summary>
        /// <value>
        /// The observable collection items.
        /// </value>
        public Dictionary<int, object> ObservableCollectionItems { get; set; }
        #endregion
        #region NotifyCollectionChangedEventHandler
        /// <summary>
        /// Gets or sets the notify collection changed event handler.
        /// </summary>
        /// <value>
        /// The notify collection changed event handler.
        /// </value>
        public NotifyCollectionChangedEventHandler NotifyCollectionChangedEventHandler { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerProperty"/> class.
        /// </summary>
        public ControllerProperty()
        {
            ObservableCollectionItems = new Dictionary<int, object>();
        }

        #endregion
    }
}
