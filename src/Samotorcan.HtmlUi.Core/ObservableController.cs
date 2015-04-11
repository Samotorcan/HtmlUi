using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Attributes;
using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Observable controller.
    /// </summary>
    public abstract class ObservableController : Controller, INotifyPropertyChanged
    {
        #region Properties
        #region Internal

        #region PropertyChanges
        /// <summary>
        /// Gets or sets the property changes.
        /// </summary>
        /// <value>
        /// The property changes.
        /// </value>
        internal HashSet<string> PropertyChanges { get; set; }
        #endregion
        #region ObservableCollectionChanges
        /// <summary>
        /// Gets or sets the observable collection changes.
        /// </summary>
        /// <value>
        /// The observable collection changes.
        /// </value>
        internal Dictionary<string, ObservableCollectionChanges> ObservableCollectionChanges { get; set; }
        #endregion
        #region ObservableControllerTypeInfo
        /// <summary>
        /// Gets or sets the observable controller type information.
        /// </summary>
        /// <value>
        /// The observable controller type information.
        /// </value>
        internal ObservableControllerTypeInfo ObservableControllerTypeInfo { get; set; }
        #endregion
        #region ObservableControllerTypeInfos
        /// <summary>
        /// Gets or sets the observable controller type infos.
        /// </summary>
        /// <value>
        /// The observable controller type infos.
        /// </value>
        internal static Dictionary<Type, ObservableControllerTypeInfo> ObservableControllerTypeInfos { get; set; }
        #endregion

        #endregion
        #region Private

        #region SyncChanges
        /// <summary>
        /// Gets or sets a value indicating whether to synchronize changes.
        /// </summary>
        /// <value>
        ///   <c>true</c> to synchronize changes; otherwise, <c>false</c>.
        /// </value>
        private bool SyncChanges { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="ObservableController"/> class.
        /// </summary>
        static ObservableController()
        {
            ObservableControllerTypeInfos = new Dictionary<Type, ObservableControllerTypeInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableController"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public ObservableController(int id)
            : base(id)
        {
            PropertyChanges = new HashSet<string>();
            ObservableCollectionChanges = new Dictionary<string, ObservableCollectionChanges>();

            LoadObservableControllerTypeInfoIfNeeded();
            ObservableControllerTypeInfo = ObservableControllerTypeInfos[Type];

            SyncChanges = true;
            PropertyChanged += PropertyChangedHandle;
        }

        #endregion
        #region Methods
        #region Internal

        #region ClearChanges
        /// <summary>
        /// Clears the changes.
        /// </summary>
        internal void ClearChanges()
        {
            PropertyChanges.Clear();
            ObservableCollectionChanges.Clear();
        }
        #endregion
        #region GetProperties
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="access">The access.</param>
        /// <returns></returns>
        internal List<ControllerPropertyDescription> GetProperties(Access access)
        {
            return ObservableControllerTypeInfo.Properties.Values.Where(p => p.Access.HasFlag(access))
                .Select(p => new ControllerPropertyDescription
                {
                    Name = p.Name,
                    Value = p.Access.HasFlag(Access.Read) ? p.GetDelegate.DynamicInvoke(this) : null
                })
                .ToList();
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
        internal List<ControllerPropertyDescription> GetProperties()
        {
            return GetProperties(Access.Read | Access.Write);
        }
        #endregion
        #region GetObservableControllerDescription
        /// <summary>
        /// Gets the observable controller description.
        /// </summary>
        /// <returns></returns>
        internal ObservableControllerDescription GetObservableControllerDescription()
        {
            return new ObservableControllerDescription
            {
                Id = Id,
                Name = Name,
                Properties = GetProperties(),
                Methods = GetMethods()
            };
        }
        #endregion
        #region SetPropertyValue
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="sync">if set to <c>true</c> synchronize changes.</param>
        /// <exception cref="System.ArgumentNullException">propertyName</exception>
        /// <exception cref="PropertyNotFoundException"></exception>
        /// <exception cref="PropertyMismatchException">
        /// null
        /// or
        /// </exception>
        internal void SetPropertyValue(string propertyName, JToken value, bool sync)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");

            var property = FindProperty(propertyName);

            if (property == null)
                throw new PropertyNotFoundException(propertyName, Name);

            if (!property.Access.HasFlag(Access.Write))
                throw new ReadOnlyPropertyException(property.Name, Name);

            if (value == null)
            {
                if (!property.PropertyType.IsValueType || Nullable.GetUnderlyingType(property.PropertyType) != null)
                    SetPropertyValueInternal(property, value, sync);
                else
                    throw new PropertyMismatchException(Name, property.Name, property.PropertyType.Name, "null");
            }
            else
            {
                try
                {
                    SetPropertyValueInternal(property, value.ToObject(property.PropertyType), sync);
                }
                catch (ArgumentException)
                {
                    throw new PropertyMismatchException(Name, property.Name, property.PropertyType.Name, Enum.GetName(typeof(JTokenType), value.Type));
                }
                catch (FormatException)
                {
                    throw new PropertyMismatchException(Name, property.Name, property.PropertyType.Name, Enum.GetName(typeof(JTokenType), value.Type));
                }
            }
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        internal void SetPropertyValue(string propertyName, JToken value)
        {
            SetPropertyValue(propertyName, value, false);
        }
        #endregion
        #region GetPropertyValue
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">propertyName</exception>
        /// <exception cref="PropertyNotFoundException"></exception>
        /// <exception cref="Samotorcan.HtmlUi.Core.Exceptions.WriteOnlyPropertyException"></exception>
        internal object GetPropertyValue(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");

            var property = FindProperty(propertyName);

            if (property == null)
                throw new PropertyNotFoundException(propertyName, Name);

            if (!property.Access.HasFlag(Access.Read))
                throw new WriteOnlyPropertyException(property.Name, Name);

            return property.GetDelegate.DynamicInvoke(this);
        }
        #endregion
        #region SetObservableCollectionChanges
        /// <summary>
        /// Sets the observable collection changes.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="sync">if set to <c>true</c> synchronize changes.</param>
        /// <exception cref="System.ArgumentNullException">
        /// propertyName
        /// or
        /// changes
        /// </exception>
        /// <exception cref="PropertyNotFoundException"></exception>
        /// <exception cref="PropertyMismatchException"></exception>
        /// <exception cref="WriteOnlyPropertyException"></exception>
        internal void SetObservableCollectionChanges(string propertyName, ObservableCollectionChanges changes, bool sync)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");

            if (changes == null)
                throw new ArgumentNullException("changes");

            var property = FindProperty(propertyName);

            if (property == null)
                throw new PropertyNotFoundException(propertyName, Name);

            if (!property.IsCollection)
                throw new PropertyMismatchException(Name, property.Name, string.Format("{0}/{1}", typeof(IList).Name, typeof(IList<>).Name), property.PropertyType.Name);

            if (!property.Access.HasFlag(Access.Read))
                throw new WriteOnlyPropertyException(property.Name, Name);

            var collection = property.GetDelegate.DynamicInvoke(this);

            if (collection != null)
            {
                if (property.IsArray)
                    SetArrayChanges((Array)collection, property, changes, sync);
                else if (property.IsIList)
                    SetIListChanges((IList)collection, property, changes, sync);
                else
                    SetGenericIListChanges(collection, property, changes, sync);
            }
        }

        /// <summary>
        /// Sets the observable collection changes.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="changes">The changes.</param>
        internal void SetObservableCollectionChanges(string propertyName, ObservableCollectionChanges changes)
        {
            SetObservableCollectionChanges(propertyName, changes, false);
        }
        #endregion

        #endregion
        #region Private

        #region LoadObservableControllerTypeInfoIfNeeded
        /// <summary>
        /// Loads the observable controller type information if needed.
        /// </summary>
        private void LoadObservableControllerTypeInfoIfNeeded()
        {
            if (!ObservableControllerTypeInfos.ContainsKey(Type))
            {
                ObservableControllerTypeInfos.Add(Type, new ObservableControllerTypeInfo
                {
                    Properties = FindProperties(Type)
                });
            }
        }
        #endregion
        #region SetPropertyValueInternal
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="sync">if set to <c>true</c> synchronize changes.</param>
        private void SetPropertyValueInternal(ControllerProperty property, object value, bool sync)
        {
            try
            {
                SyncChanges = sync;

                property.SetDelegate.DynamicInvoke(this, value);
            }
            finally
            {
                SyncChanges = true;
            }
        }
        #endregion
        #region FindProperties
        /// <summary>
        /// Finds the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private Dictionary<string, ControllerProperty> FindProperties(Type type)
        {
            var properties = new Dictionary<string, ControllerProperty>();

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // ignore properties with exclude attribute
                if (property.GetCustomAttribute<ExcludeAttribute>() == null)
                {
                    var getMethod = property.GetGetMethod(false);
                    var setMethod = property.GetSetMethod(false);
                    Access? access = null;

                    // access
                    if (property.CanRead && getMethod != null && property.CanWrite && setMethod != null)
                        access = Access.Read | Access.Write;
                    else if (property.CanRead && getMethod != null)
                        access = Access.Read;
                    else if (property.CanWrite && setMethod != null)
                        access = Access.Write;

                    // add
                    if (access != null)
                    {
                        var controllerProperty = new ControllerProperty
                        {
                            Name = property.Name,
                            PropertyType = property.PropertyType,
                            IsObservableCollection = IsObservableCollection(property.PropertyType),
                            IsCollection = IsCollection(property.PropertyType),
                            IsIList = IsIList(property.PropertyType),
                            IsGenericIList = IsGenericIList(property.PropertyType),
                            IsArray = property.PropertyType.IsArray,
                            GetDelegate = getMethod != null ? ExpressionUtility.CreateMethodDelegate(getMethod) : null,
                            SetDelegate = setMethod != null ? ExpressionUtility.CreateMethodDelegate(setMethod) : null,
                            Access = access.Value
                        };

                        if (controllerProperty.IsArray)
                            AddArrayInfo(controllerProperty);

                        if (controllerProperty.IsGenericIList)
                            AddGenericIListInfo(controllerProperty);

                        if (controllerProperty.IsObservableCollection)
                            AddObservableCollection(controllerProperty);

                        properties.Add(controllerProperty.Name, controllerProperty);
                    }
                }
            }

            return properties;
        }
        #endregion
        #region IsObservableCollection
        /// <summary>
        /// Determines whether type is observable collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private bool IsObservableCollection(Type type)
        {
            return typeof(INotifyCollectionChanged).IsAssignableFrom(type) && IsCollection(type);
        }
        #endregion
        #region IsCollection
        /// <summary>
        /// Determines whether the specified type is collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private bool IsCollection(Type type)
        {
            return (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)) ||
                    typeof(IList).IsAssignableFrom(type));
        }
        #endregion
        #region IsIList
        /// <summary>
        /// Determines whether the specified type is IsIList.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private bool IsIList(Type type)
        {
            return typeof(IList).IsAssignableFrom(type) && !type.IsArray;
        }
        #endregion
        #region IsGenericIList
        /// <summary>
        /// Determines whether the specified type is generic IList.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private bool IsGenericIList(Type type)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)) && !type.IsArray;
        }
        #endregion
        #region AddGenericIListInfo
        /// <summary>
        /// Adds the generic IList info.
        /// </summary>
        /// <param name="property">The property.</param>
        private void AddGenericIListInfo(ControllerProperty property)
        {
            property.GenericIListAddDelegate = ExpressionUtility.CreateMethodDelegate(property.PropertyType.GetMethod("Add"));
            property.GenericIListRemoveAtDelegate = ExpressionUtility.CreateMethodDelegate(property.PropertyType.GetMethod("RemoveAt"));
            property.GenericIListCountDelegate = ExpressionUtility.CreateMethodDelegate(property.PropertyType.GetProperty("Count").GetGetMethod());
            property.GenericIListInsertDelegate = ExpressionUtility.CreateMethodDelegate(property.PropertyType.GetMethod("Insert"));

            var indexMethod = property.PropertyType.GetProperties()
                .Single(p => p.GetIndexParameters().Length == 1 && p.GetIndexParameters()[0].ParameterType == typeof(int))
                .GetSetMethod(false);

            property.GenericIListReplaceDelegate = ExpressionUtility.CreateMethodDelegate(indexMethod);

            property.GenericIListType = property.PropertyType.GetGenericArguments()[0];
        }
        #endregion
        #region AddArrayInfo
        /// <summary>
        /// Adds the array information.
        /// </summary>
        /// <param name="property">The property.</param>
        private void AddArrayInfo(ControllerProperty property)
        {
            property.ArrayType = property.PropertyType.GetElementType();
        }
        #endregion
        #region PadIList
        /// <summary>
        /// Pads the IList.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="toIndex">To index.</param>
        /// <param name="itemType">Type of the item.</param>
        private void PadIList(IList list, int toIndex, Type itemType)
        {
            while (list.Count < toIndex)
                list.Add(GetDefaultValue(itemType));
        }
        #endregion
        #region PadGenericIList
        /// <summary>
        /// Pads the generic IList.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="property">The property.</param>
        /// <param name="toIndex">To index.</param>
        private void PadGenericIList(object list, ControllerProperty property, int toIndex)
        {
            while (property.GenericIListCount(list) < toIndex)
                property.GenericIListAdd(list, GetDefaultValue(property.GenericIListType));
        }
        #endregion
        #region GetDefaultValue
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }
        #endregion
        #region SetIListChanges
        /// <summary>
        /// Sets the IList changes.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="property">The property.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="sync">if set to <c>true</c> synchronize changes.</param>
        private void SetIListChanges(IList list, ControllerProperty property, ObservableCollectionChanges changes, bool sync)
        {
            try
            {
                SyncChanges = sync;

                var itemType = property.IsGenericIList ? property.GenericIListType : typeof(object);

                foreach (var change in changes.Actions)
                {
                    if (change.Action == ObservableCollectionChangeAction.Add)
                    {
                        var startIndex = change.NewStartingIndex.Value;

                        foreach (var item in change.NewItems)
                        {
                            PadIList(list, startIndex, itemType);

                            if (startIndex >= list.Count)
                                list.Add(item.ToObject(itemType));
                            else
                                list.Insert(startIndex, item.ToObject(itemType));

                            startIndex++;
                        }
                    }
                    else if (change.Action == ObservableCollectionChangeAction.Remove)
                    {
                        var removeIndex = change.OldStartingIndex.Value;

                        if (list.Count > removeIndex)
                            list.RemoveAt(removeIndex);
                    }
                    else if (change.Action == ObservableCollectionChangeAction.Replace)
                    {
                        var startIndex = change.NewStartingIndex.Value;

                        foreach (var item in change.NewItems)
                        {
                            PadIList(list, startIndex, itemType);

                            if (startIndex >= list.Count)
                                list.Add(item.ToObject(itemType));
                            else
                                list[startIndex] = item.ToObject(itemType);

                            startIndex++;
                        }
                    }
                }
            }
            finally
            {
                SyncChanges = true;
            }
        }
        #endregion
        #region SetArrayChanges
        /// <summary>
        /// Sets the Array changes.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="property">The property.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="sync">if set to <c>true</c> synchronize changes.</param>
        private void SetArrayChanges(Array array, ControllerProperty property, ObservableCollectionChanges changes, bool sync)
        {
            // TODO; fix throw exceptions

            try
            {
                SyncChanges = sync;

                foreach (var change in changes.Actions)
                {
                    if (change.Action == ObservableCollectionChangeAction.Add)
                    {
                        throw new PropertyMismatchException(Name, property.Name, string.Format("{0}/{1}", typeof(IList).Name, typeof(IList<>).Name), property.PropertyType.Name);
                    }
                    else if (change.Action == ObservableCollectionChangeAction.Remove)
                    {
                        throw new PropertyMismatchException(Name, property.Name, string.Format("{0}/{1}", typeof(IList).Name, typeof(IList<>).Name), property.PropertyType.Name);
                    }
                    else if (change.Action == ObservableCollectionChangeAction.Replace)
                    {
                        var startIndex = change.NewStartingIndex.Value;

                        foreach (var item in change.NewItems)
                        {
                            if (startIndex >= array.Length)
                                throw new PropertyMismatchException(Name, property.Name, string.Format("{0}/{1}", typeof(IList).Name, typeof(IList<>).Name), property.PropertyType.Name);

                            array.SetValue(item.ToObject(property.ArrayType), startIndex);

                            startIndex++;
                        }
                    }
                }
            }
            finally
            {
                SyncChanges = true;
            }
        }
        #endregion
        #region SetGenericIListChanges
        /// <summary>
        /// Sets the generic IList changes.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="property">The property.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="sync">if set to <c>true</c> synchronize changes.</param>
        private void SetGenericIListChanges(object list, ControllerProperty property, ObservableCollectionChanges changes, bool sync)
        {
            try
            {
                SyncChanges = sync;
                foreach (var change in changes.Actions)
                {
                    if (change.Action == ObservableCollectionChangeAction.Add)
                    {
                        var startIndex = change.NewStartingIndex.Value;

                        foreach (var item in change.NewItems)
                        {
                            PadGenericIList(list, property, startIndex);

                            if (startIndex >= property.GenericIListCount(list))
                                property.GenericIListAdd(list, item.ToObject(property.GenericIListType));
                            else
                                property.GenericIListInsert(list, startIndex, item.ToObject(property.GenericIListType));

                            startIndex++;
                        }
                    }
                    else if (change.Action == ObservableCollectionChangeAction.Remove)
                    {
                        var removeIndex = change.OldStartingIndex.Value;

                        if (property.GenericIListCount(list) > removeIndex)
                            property.GenericIListRemoveAt(list, removeIndex);
                    }
                    else if (change.Action == ObservableCollectionChangeAction.Replace)
                    {
                        var startIndex = change.NewStartingIndex.Value;

                        foreach (var item in change.NewItems)
                        {
                            PadGenericIList(list, property, startIndex);

                            if (startIndex >= property.GenericIListCount(list))
                                property.GenericIListAdd(list, item.ToObject(property.GenericIListType));
                            else
                                property.GenericIListReplace(list, startIndex, item.ToObject(property.GenericIListType));

                            startIndex++;
                        }
                    }
                }
            }
            finally
            {
                SyncChanges = true;
            }
        }
        #endregion
        #region FindProperty
        /// <summary>
        /// Finds the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private ControllerProperty FindProperty(string propertyName)
        {
            ControllerProperty property = null;

            if (ObservableControllerTypeInfo.Properties.TryGetValue(propertyName, out property))
                return property;

            if (ObservableControllerTypeInfo.Properties.TryGetValue(StringUtility.PascalCase(propertyName), out property))
                return property;

            if (ObservableControllerTypeInfo.Properties.TryGetValue(StringUtility.CamelCase(propertyName), out property))
                return property;

            return null;
        }
        #endregion
        #region AddObservableCollection
        /// <summary>
        /// Adds the observable collection.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        private void AddObservableCollection(ControllerProperty property)
        {
            RemoveObservableCollection(property);

            property.ObservableCollection = (INotifyCollectionChanged)property.GetDelegate.DynamicInvoke(this);

            if (property.ObservableCollection != null)
            {
                property.NotifyCollectionChangedEventHandler = new NotifyCollectionChangedEventHandler((sender, e) => { CollectionChangedHandle(sender, e, property); });
                property.ObservableCollection.CollectionChanged += property.NotifyCollectionChangedEventHandler;
            }
        }
        #endregion
        #region RemoveObservableCollection
        /// <summary>
        /// Removes the observable collection.
        /// </summary>
        /// <param name="property">The property.</param>
        private void RemoveObservableCollection(ControllerProperty property)
        {
            if (property.ObservableCollection != null)
                property.ObservableCollection.CollectionChanged -= property.NotifyCollectionChangedEventHandler;

            property.NotifyCollectionChangedEventHandler = null;
            property.ObservableCollection = null;
        }
        #endregion

        #region PropertyChangedHandle
        /// <summary>
        /// Property changed handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void PropertyChangedHandle(object sender, PropertyChangedEventArgs e)
        {
            var property = FindProperty(e.PropertyName);

            if (property.IsObservableCollection)
                AddObservableCollection(property);

            if (SyncChanges)
                PropertyChanges.Add(e.PropertyName);
        }
        #endregion
        #region CollectionChangedHandle
        /// <summary>
        /// Collection changed handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="property">The property.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sender", Justification = "I want it to be the same as the event parameters.")]
        private void CollectionChangedHandle(object sender, NotifyCollectionChangedEventArgs e, ControllerProperty property)
        {
            if (!SyncChanges)
                return;

            if (!ObservableCollectionChanges.ContainsKey(property.Name))
                ObservableCollectionChanges.Add(property.Name, new ObservableCollectionChanges { Name = property.Name });

            var changes = ObservableCollectionChanges[property.Name];

            if (!changes.IsReset)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        changes.Actions.Add(new ObservableCollectionChange
                        {
                            NewItems = new JArray(e.NewItems),
                            NewStartingIndex = e.NewStartingIndex,
                            Action = ObservableCollectionChangeAction.Add
                        });
                        break;
                    case NotifyCollectionChangedAction.Move:
                        changes.Actions.Add(new ObservableCollectionChange
                        {
                            OldStartingIndex = e.OldStartingIndex,
                            NewStartingIndex = e.NewStartingIndex,
                            Action = ObservableCollectionChangeAction.Move
                        });
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        changes.Actions.Add(new ObservableCollectionChange
                        {
                            OldStartingIndex = e.OldStartingIndex,
                            Action = ObservableCollectionChangeAction.Remove
                        });
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        changes.Actions.Add(new ObservableCollectionChange
                        {
                            NewItems = new JArray(e.NewItems),
                            NewStartingIndex = e.NewStartingIndex,
                            Action = ObservableCollectionChangeAction.Replace
                        });
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        changes.Actions = new List<ObservableCollectionChange>();
                        changes.IsReset = true;
                        break;
                }
            }
        }
        #endregion

        #endregion
        #endregion

        #region IDisposable

        /// <summary>
        /// Was dispose already called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!_disposed)
            {
                if (disposing)
                {
                    PropertyChanged -= PropertyChangedHandle;

                    foreach (var property in ObservableControllerTypeInfo.Properties.Values)
                    {
                        if (property.ObservableCollection != null && property.NotifyCollectionChangedEventHandler != null)
                            property.ObservableCollection.CollectionChanged -= property.NotifyCollectionChangedEventHandler;
                    }
                }

                _disposed = true;
            }
        }

        #endregion
        #region INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Used for CallerMemberName.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "It must be a ref field.")]
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            BaseMainApplication.Current.EnsureMainThread();

            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
