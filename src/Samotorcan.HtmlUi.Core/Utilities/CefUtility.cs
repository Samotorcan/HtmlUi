using Samotorcan.HtmlUi.Core.Logs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// CEF utility.
    /// </summary>
    internal static class CefUtility
    {
        #region Properties
        #region Private

        #region TypeProperties
        /// <summary>
        /// Gets or sets the type properties.
        /// </summary>
        /// <value>
        /// The type properties.
        /// </value>
        private static Dictionary<Type, List<PropertyInfo>> TypeProperties { get; set; }
        #endregion
        #region CollectionAddMethod
        /// <summary>
        /// Gets or sets the collection add method.
        /// </summary>
        /// <value>
        /// The collection add method.
        /// </value>
        private static MethodInfo CollectionAddMethod { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="CefUtility"/> class.
        /// </summary>
        static CefUtility()
        {
            TypeProperties = new Dictionary<Type, List<PropertyInfo>>();
            CollectionAddMethod = typeof(Collection<>).GetMethod("Add");
        }

        #endregion
        #region Methods
        #region Public

        #region ExecuteTask
        /// <summary>
        /// Executes the task on the selected CEF thread.
        /// </summary>
        /// <param name="threadId">The thread identifier.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public static void ExecuteTask(CefThreadId threadId, Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (!CefRuntime.CurrentlyOn(threadId))
                CefRuntime.PostTask(threadId, new ActionTask(action));
            else
                action();
        }
        #endregion
        #region ParseValue
        /// <summary>
        /// Parses the value.   TODO: not done
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <param name="propertyNameType">Type of the property name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// value
        /// or
        /// type
        /// </exception>
        /// <exception cref="System.InvalidCastException">
        /// Invalid type.
        /// </exception>
        private static object ParseValue(CefV8Value value, Type type, NormalizeType propertyNameType)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (type == null)
                throw new ArgumentNullException("type");

            GeneralLog.Debug(string.Format("Parse value: {0}", type.Name));

            // simple values
            if (value.IsUndefined || value.IsNull)
            {
                if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null || type == typeof(object))
                    return null;

                throw new InvalidCastException("Invalid type.");
            }
            else if (value.IsString)
            {
                if (type == typeof(string) || type == typeof(object))
                    return value.GetStringValue();

                throw new InvalidCastException("Invalid type.");
            }
            else if (value.IsInt)
            {
                if (type == typeof(int) || type == typeof(object))
                    return value.GetIntValue();

                throw new InvalidCastException("Invalid type.");
            }
            else if (value.IsUInt)
            {
                if ((type == typeof(int) || type == typeof(object)) && value.GetUIntValue() <= int.MaxValue)
                    return value.GetIntValue();

                if (type == typeof(uint) || type == typeof(object))
                    return value.GetUIntValue();

                throw new InvalidCastException("Invalid type.");
            }
            else if (value.IsBool)
            {
                if (type == typeof(bool) || type == typeof(object))
                    return value.GetBoolValue();

                throw new InvalidCastException("Invalid type.");
            }
            else if (value.IsDate)
            {
                if (type == typeof(DateTime) || type == typeof(object))
                    return value.GetDateValue();

                throw new InvalidCastException("Invalid type.");
            }
            else if (value.IsDouble)
            {
                if (type == typeof(double) || type == typeof(object))
                    return value.GetDoubleValue();

                throw new InvalidCastException("Invalid type.");
            }
            else if (value.IsArray)
            {
                if (type.IsArray)
                {
                    var itemType = type.GetElementType();
                    var array = Array.CreateInstance(itemType, value.GetArrayLength());

                    for (var i = 0; i < value.GetArrayLength(); i++)
                        array.SetValue(ParseValue(value.GetValue(i), itemType, propertyNameType), i);

                    return array;
                }
                else if (IsCollection(type))
                {
                    var itemType = type.GetGenericArguments()[0];
                    var collection = CreateCollection(itemType);

                    for (var i = 0; i < value.GetArrayLength(); i++)
                        AddCollectionItem(collection, ParseValue(value.GetValue(i), itemType, propertyNameType));

                    return collection;
                }
                
                throw new InvalidCastException("Invalid type.");
            }
            else if (value.IsObject)
            {
                var typeObject = Activator.CreateInstance(type);
                List<PropertyInfo> properties = GetProperties(type);

                foreach (var property in properties)
                {
                    var propertyName = StringUtility.Normalize(property.Name, propertyNameType);

                    property.SetValue(typeObject, ParseValue(value.GetValue(propertyName), property.PropertyType, propertyNameType));
                }

                return typeObject;
            }

            throw new InvalidCastException("Invalid type.");
        }

        /// <summary>
        /// Parses the value.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyNameType">Type of the property name.</param>
        /// <returns></returns>
        public static TType ParseValue<TType>(CefV8Value value, NormalizeType propertyNameType)
        {
            return (TType)CefUtility.ParseValue(value, typeof(TType), propertyNameType);
        }

        /// <summary>
        /// Parses the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object ParseValue(CefV8Value value, Type type)
        {
            return CefUtility.ParseValue(value, type, NormalizeType.CamelCase);
        }

        /// <summary>
        /// Parses the value.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static TType ParseValue<TType>(CefV8Value value)
        {
            return (TType)CefUtility.ParseValue(value, typeof(TType), NormalizeType.CamelCase);
        }
        #endregion

        #endregion
        #region Private

        #region GetProperties
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static List<PropertyInfo> GetProperties(Type type)
        {
            if (!TypeProperties.ContainsKey(type))
            {
                List<PropertyInfo> properties = new List<PropertyInfo>();
                TypeProperties.Add(type, properties);

                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.CanWrite && property.GetSetMethod(false) != null)
                        properties.Add(property);
                }
            }

            return TypeProperties[type];
        }
        #endregion
        #region IsCollection
        /// <summary>
        /// Determines whether the specified type is collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static bool IsCollection(Type type)
        {
            return type.GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
        }
        #endregion
        #region CreateCollection
        /// <summary>
        /// Creates the collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static ICollection CreateCollection(Type type)
        {
            var genericListType = typeof(Collection<>).MakeGenericType(new[] { type });

            return (ICollection)Activator.CreateInstance(genericListType);
        }
        #endregion
        #region AddCollectionItem
        /// <summary>
        /// Adds the collection item.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        private static void AddCollectionItem(ICollection collection, object item)
        {
            CollectionAddMethod.Invoke(collection, new[] { item });
        }
        #endregion

        #endregion
        #endregion
    }
}
