using System;
using System.Collections.Specialized;

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
        #region IsCollection
        /// <summary>
        /// Gets or sets a value indicating whether this instance is collection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollection { get; set; }
        #endregion
        #region IsIList
        /// <summary>
        /// Gets or sets a value indicating whether this instance is IList.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is IList; otherwise, <c>false</c>.
        /// </value>
        public bool IsIList { get; set; }
        #endregion
        #region IsGenericIList
        /// <summary>
        /// Gets or sets a value indicating whether this instance is generic IList.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is generic IList; otherwise, <c>false</c>.
        /// </value>
        public bool IsGenericIList { get; set; }
        #endregion
        #region IsArray
        /// <summary>
        /// Gets or sets a value indicating whether this instance is array.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
        /// </value>
        public bool IsArray { get; set; }
        #endregion
        #region ArrayType
        /// <summary>
        /// Gets or sets the type of the array.
        /// </summary>
        /// <value>
        /// The type of the array.
        /// </value>
        public Type ArrayType { get; set; }
        #endregion
        #region GenericIListAddDelegate
        /// <summary>
        /// Gets or sets the generic IList add delegate.
        /// </summary>
        /// <value>
        /// The generic IList add delegate.
        /// </value>
        public Delegate GenericIListAddDelegate { get; set; }
        #endregion
        #region GenericIListRemoveAtDelegate
        /// <summary>
        /// Gets or sets the generic IList remove at delegate.
        /// </summary>
        /// <value>
        /// The generic IList remove at delegate.
        /// </value>
        public Delegate GenericIListRemoveAtDelegate { get; set; }
        #endregion
        #region GenericIListReplaceDelegate
        /// <summary>
        /// Gets or sets the generic IList replace delegate.
        /// </summary>
        /// <value>
        /// The generic IList replace delegate.
        /// </value>
        public Delegate GenericIListReplaceDelegate { get; set; }
        #endregion
        #region GenericIListCountDelegate
        /// <summary>
        /// Gets or sets the generic IList count delegate.
        /// </summary>
        /// <value>
        /// The generic IList count delegate.
        /// </value>
        public Delegate GenericIListCountDelegate { get; set; }
        #endregion
        #region GenericIListInsertDelegate
        /// <summary>
        /// Gets or sets the generic IList insert delegate.
        /// </summary>
        /// <value>
        /// The generic IList insert delegate.
        /// </value>
        public Delegate GenericIListInsertDelegate { get; set; }
        #endregion
        #region GenericIListType
        /// <summary>
        /// Gets or sets the type of the generic IList.
        /// </summary>
        /// <value>
        /// The type of the generic IList.
        /// </value>
        public Type GenericIListType { get; set; }
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
        #region Methods
        #region Public

        #region GenericIListCount
        /// <summary>
        /// Generic IList count.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public int GenericIListCount(object list)
        {
            return (int)GenericIListCountDelegate.DynamicInvoke(list);
        }
        #endregion
        #region GenericIListInsert
        /// <summary>
        /// Generic IList insert.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public void GenericIListInsert(object list, int index, object item)
        {
            GenericIListInsertDelegate.DynamicInvoke(list, index, item);
        }
        #endregion
        #region GenericIListReplace
        /// <summary>
        /// Generic IList replace.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public void GenericIListReplace(object list, int index, object item)
        {
            GenericIListReplaceDelegate.DynamicInvoke(list, index, item);
        }
        #endregion
        #region GenericIListRemoveAt
        /// <summary>
        /// Generic IList remove at.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        public void GenericIListRemoveAt(object list, int index)
        {
            GenericIListRemoveAtDelegate.DynamicInvoke(list, index);
        }
        #endregion
        #region GenericIListAdd
        /// <summary>
        /// Generic IList add.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item.</param>
        public void GenericIListAdd(object list, object item)
        {
            GenericIListAddDelegate.DynamicInvoke(list, item);
        }
        #endregion

        #endregion
        #endregion
    }
}
