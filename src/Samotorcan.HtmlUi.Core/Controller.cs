using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Attributes;
using Samotorcan.HtmlUi.Core.Diagnostics;
using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller. TODO: create attributes like controller name, camel case ...
    /// </summary>
    public abstract class Controller : IDisposable, INotifyPropertyChanged
    {
        #region Properties
        #region Public

        #region Id
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Exclude]
        public int Id { get; private set; }
        #endregion
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Exclude]
        public string Name { get; private set; }
        #endregion

        #endregion
        #region Internal

        #region PropertyChanges
        /// <summary>
        /// Gets or sets the property changes.
        /// </summary>
        /// <value>
        /// The property changes.
        /// </value>
        internal HashSet<string> PropertyChanges { get; private set; }
        #endregion

        #endregion
        #region Private

        #region ControllerTypeInfo
        /// <summary>
        /// Gets or sets the controller type information.
        /// </summary>
        /// <value>
        /// The controller type information.
        /// </value>
        private ControllerTypeInfo ControllerTypeInfo { get; set; }
        #endregion
        #region Type
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        private Type Type { get; set; }
        #endregion
        #region SyncPropertyValue
        /// <summary>
        /// Gets or sets a value indicating whether to synchronize property value.
        /// </summary>
        /// <value>
        /// <c>true</c> to synchronize property value; otherwise, <c>false</c>.
        /// </value>
        private bool SyncPropertyValue { get; set; }
        #endregion

        #region ControllerTypeInfos
        /// <summary>
        /// Gets or sets the controller type informations.
        /// </summary>
        /// <value>
        /// The controller type informations.
        /// </value>
        private static Dictionary<Type, ControllerTypeInfo> ControllerTypeInfos { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="Controller"/> class.
        /// </summary>
        static Controller()
        {
            ControllerTypeInfos = new Dictionary<Type, ControllerTypeInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        protected Controller(int id)
        {
            Id = id;

            PropertyChanges = new HashSet<string>();

            Type = GetType();
            Name = Type.Name;

            // load controller type info if needed
            if (!ControllerTypeInfos.ContainsKey(Type))
            {
                ControllerTypeInfos.Add(Type, new ControllerTypeInfo
                {
                    Properties = FindProperties(Type),
                    Methods = FindMethods(Type),
                    InternalMethods = FindInternalMethods()
                });
            }

            ControllerTypeInfo = ControllerTypeInfos[Type];

            SyncPropertyValue = true;
            PropertyChanged += Controller_PropertyChanged;
        }

        #endregion
        #region Methods
        #region Internal

        #region GetPropertyDescriptions
        /// <summary>
        /// Gets the property descriptions.
        /// </summary>
        /// <param name="access">The access.</param>
        /// <param name="normalizeType">The normalize type.</param>
        /// <returns></returns>
        private List<ControllerPropertyDescription> GetPropertyDescriptions(Access? access, NormalizeType normalizeType)
        {
            return ControllerTypeInfo.Properties.Where(p => access == null || p.Access.HasFlag(access.Value))
                .Select(p => new ControllerPropertyDescription
                {
                    Name = StringUtility.Normalize(p.Name, normalizeType),
                    Value = p.Access.HasFlag(Access.Read) ? p.GetDelegate.DynamicInvoke(this) : null
                })
                .ToList();
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="access">The access.</param>
        /// <param name="normalizeType">The normalize type.</param>
        /// <returns></returns>
        internal List<ControllerPropertyDescription> GetProperties(Access access, NormalizeType normalizeType)
        {
            return GetPropertyDescriptions((Access?)access, normalizeType);
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="normalizeType">The normalize type.</param>
        /// <returns></returns>
        internal List<ControllerPropertyDescription> GetProperties(NormalizeType normalizeType)
        {
            return GetPropertyDescriptions(null, normalizeType);
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
        internal List<ControllerPropertyDescription> GetProperties()
        {
            return GetPropertyDescriptions(null, NormalizeType.CamelCase);
        }
        #endregion
        #region GetMethodDescriptions
        /// <summary>
        /// Gets the method descriptions.
        /// </summary>
        /// <param name="normalizeType">The normalize type.</param>
        /// <returns></returns>
        internal List<ControllerMethodDescription> GetMethodDescriptions(NormalizeType normalizeType)
        {
            return ControllerTypeInfo.Methods.Select(m => new ControllerMethodDescription
            {
                Name = StringUtility.Normalize(m.Name, normalizeType)
            })
            .ToList();
        }

        /// <summary>
        /// Gets the method descriptions.
        /// </summary>
        /// <returns></returns>
        internal List<ControllerMethodDescription> GetMethodDescriptions()
        {
            return GetMethodDescriptions(NormalizeType.CamelCase);
        }
        #endregion
        #region GetDescription
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="normalizeType">The normalize type.</param>
        /// <returns></returns>
        internal ControllerDescription GetDescription(NormalizeType normalizeType)
        {
            return new ControllerDescription
            {
                Name = Name,
                Properties = GetProperties(normalizeType),
                Methods = GetMethodDescriptions(normalizeType)
            };
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        internal ControllerDescription GetDescription()
        {
            return GetDescription(NormalizeType.CamelCase);
        }
        #endregion
        #region SetPropertyValue
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="sync">if set to <c>true</c> synchronize changes.</param>
        /// <param name="normalizeType">Type of the normalize.</param>
        /// <exception cref="System.ArgumentNullException">propertyName</exception>
        /// <exception cref="PropertyNotFoundException"></exception>
        /// <exception cref="PropertyMismatchException">
        /// null
        /// or
        /// </exception>
        internal void SetPropertyValue(string propertyName, JToken value, bool sync, NormalizeType normalizeType)
        {
            Stopwatch.Measure(() => {
                if (string.IsNullOrWhiteSpace(propertyName))
                    throw new ArgumentNullException("propertyName");

                var property = ControllerTypeInfo.Properties.FirstOrDefault(p => p.Access.HasFlag(Access.Write) &&
                    StringUtility.Normalize(p.Name, normalizeType) == propertyName);

                if (property == null)
                    throw new PropertyNotFoundException(propertyName, Name);

                if (value == null)
                {
                    if (!property.PropertyType.IsValueType || Nullable.GetUnderlyingType(property.PropertyType) != null)
                        SetPropertyValue(property, value, sync);
                    else
                        throw new PropertyMismatchException(Name, property.Name, property.PropertyType.Name, "null");
                }
                else
                {
                    try
                    {
                        SetPropertyValue(property, value.ToObject(property.PropertyType), sync);
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
            });
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="sync">if set to <c>true</c> synchronize changes.</param>
        internal void SetPropertyValue(string propertyName, JToken value, bool sync)
        {
            SetPropertyValue(propertyName, value, sync, NormalizeType.CamelCase);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        internal void SetPropertyValue(string propertyName, JToken value)
        {
            SetPropertyValue(propertyName, value, false, NormalizeType.CamelCase);
        }
        #endregion
        #region GetPropertyValue
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="normalizeType">The normalize type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">propertyName</exception>
        /// <exception cref="PropertyNotFoundException"></exception>
        /// <exception cref="Samotorcan.HtmlUi.Core.Exceptions.WriteOnlyPropertyException"></exception>
        internal object GetPropertyValue(string propertyName, NormalizeType normalizeType)
        {
            return Stopwatch.Measure(() => {
                if (string.IsNullOrWhiteSpace(propertyName))
                    throw new ArgumentNullException("propertyName");

                propertyName = StringUtility.Normalize(propertyName, normalizeType);

                var property = ControllerTypeInfo.Properties.SingleOrDefault(p => p.Name == propertyName);

                if (property == null)
                    throw new PropertyNotFoundException(propertyName, Name);

                if (!property.Access.HasFlag(Access.Read))
                    throw new WriteOnlyPropertyException(property.Name, Name);

                return property.GetDelegate.DynamicInvoke(this);
            });
        }

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
            return GetPropertyValue(propertyName, NormalizeType.PascalCase);
        }
        #endregion
        #region CallMethod
        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="internalMethod">if set to <c>true</c> [internal method].</param>
        /// <param name="normalizeType">Type of the normalize.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        /// <exception cref="MethodNotFoundException"></exception>
        /// <exception cref="ParameterCountMismatchException"></exception>
        internal object CallMethod(string name, JArray arguments, bool internalMethod, NormalizeType normalizeType)
        {
            return Stopwatch.Measure(() => {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException("name");

                if (arguments == null)
                    arguments = new JArray();

                var method = (internalMethod ? ControllerTypeInfo.InternalMethods : ControllerTypeInfo.Methods).FirstOrDefault(m => StringUtility.Normalize(m.Name, normalizeType) == name);

                if (method == null)
                    throw new MethodNotFoundException(name, Name);

                if (method.ParameterTypes.Count != arguments.Count)
                    throw new ParameterCountMismatchException(method.Name, Name);

                // parse parameters
                var parameters = method.ParameterTypes
                    .Select((t, i) =>
                    {
                        try
                        {
                            return arguments[i].ToObject(t);
                        }
                        catch (FormatException)
                        {
                            throw new ParameterMismatchException(i, t.Name, Enum.GetName(typeof(JTokenType), arguments[i].Type), method.Name, Name);
                        }
                    })
                    .ToList();

                // return result
                var delegateParameters = new List<object> { this };
                delegateParameters.AddRange(parameters);

                if (method.MethodType == MethodType.Action)
                {
                    method.Delegate.DynamicInvoke(delegateParameters.ToArray());

                    return Undefined.Value;
                }
                else
                {
                    return method.Delegate.DynamicInvoke(delegateParameters.ToArray());
                }
            });
        }

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="internalMethod">if set to <c>true</c> [internal method].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        /// <exception cref="MethodNotFoundException"></exception>
        /// <exception cref="ParameterCountMismatchException"></exception>
        internal object CallMethod(string name, JArray arguments, bool internalMethod)
        {
            return CallMethod(name, arguments, internalMethod, NormalizeType.CamelCase);
        }

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        /// <exception cref="MethodNotFoundException"></exception>
        /// <exception cref="ParameterCountMismatchException"></exception>
        internal object CallMethod(string name, JArray arguments)
        {
            return CallMethod(name, arguments, false, NormalizeType.CamelCase);
        }
        #endregion

        #endregion
        #region Private

        #region SetPropertyValue
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="sync">if set to <c>true</c> [synchronize].</param>
        private void SetPropertyValue(ControllerProperty property, object value, bool sync)
        {
            try
            {
                SyncPropertyValue = sync;

                property.SetDelegate.DynamicInvoke(this, value);
            }
            finally
            {
                SyncPropertyValue = true;
            }
        }
        #endregion
        #region IsValidMethod
        /// <summary>
        /// Determines whether the method is valid.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="methodInfos">The method infos.</param>
        /// <returns></returns>
        private bool IsValidMethod(MethodInfo methodInfo, IEnumerable<MethodInfo> methodInfos)
        {
            bool isValid = true;

            if (methodInfos.Count(m => m.Name == methodInfo.Name) > 1)
            {
                GeneralLog.Warn(string.Format("Overloaded methods are not supported. (controller = \"{0}\", method = \"{1}\")", Name, methodInfo.Name));
                isValid = false;
            }
            else if (methodInfo.GetParameters().Any(p => p.ParameterType.IsByRef))
            {
                GeneralLog.Warn(string.Format("Ref parameters are not supported. (controller = \"{0}\", method = \"{1}\")", Name, methodInfo.Name));
                isValid = false;
            }
            else if (methodInfo.GetParameters().Any(p => p.IsOut))
            {
                GeneralLog.Warn(string.Format("Out parameters are not supported. (controller = \"{0}\", method = \"{1}\")", Name, methodInfo.Name));
                isValid = false;
            }
            else if (methodInfo.IsGenericMethod)
            {
                GeneralLog.Warn(string.Format("Generic methods are not supported. (controller = \"{0}\", method = \"{1}\")", Name, methodInfo.Name));
                isValid = false;
            }

            return isValid;
        }
        #endregion
        #region MethodInfoToControllerMethod
        /// <summary>
        /// Method info to controller method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        private ControllerMethod MethodInfoToControllerMethod(MethodInfo method)
        {
            return new ControllerMethod
            {
                Name = method.Name,
                Delegate = ExpressionUtility.CreateMethodDelegate(method),
                MethodType = method.ReturnType == typeof(void) ? MethodType.Action : MethodType.Function,
                ParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToList()
            };
        }
        #endregion
        #region WarmUp
        /// <summary>
        /// Warms up the native calls.
        /// </summary>
        /// <param name="warmUp">The warm up.</param>
        /// <returns></returns>
        [InternalMethod]
        private object WarmUp(string warmUp)
        {
            return new object();
        }
        #endregion

        #region FindProperties
        /// <summary>
        /// Finds the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private List<ControllerProperty> FindProperties(Type type)
        {
            return Stopwatch.Measure(() =>
            {
                var properties = new List<ControllerProperty>();

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
                            properties.Add(new ControllerProperty
                            {
                                Name = property.Name,
                                PropertyType = property.PropertyType,
                                GetDelegate = getMethod != null ? ExpressionUtility.CreateMethodDelegate(getMethod) : null,
                                SetDelegate = setMethod != null ? ExpressionUtility.CreateMethodDelegate(setMethod) : null,
                                Access = access.Value
                            });
                        }
                    }
                }

                return properties;
            });
        }
        #endregion
        #region FindMethods
        /// <summary>
        /// Finds the methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private List<ControllerMethod> FindMethods(Type type)
        {
            return Stopwatch.Measure(() =>
            {
                var methods = new List<ControllerMethod>();

                while (type != typeof(object))
                {
                    var methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(m => !m.IsSpecialName && m.GetCustomAttribute<ExcludeAttribute>() == null)
                        .ToList();

                    foreach (var methodInfo in methodInfos)
                    {
                        if (IsValidMethod(methodInfo, methodInfos))
                            methods.Add(MethodInfoToControllerMethod(methodInfo));
                    }

                    type = type.BaseType;
                }

                return methods;
            });
        }
        #endregion
        #region FindInternalMethods
        /// <summary>
        /// Finds the internal methods.
        /// </summary>
        /// <returns></returns>
        private List<ControllerMethod> FindInternalMethods()
        {
            return Stopwatch.Measure(() =>
            {
                return typeof(Controller).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName && m.GetCustomAttribute<InternalMethodAttribute>() != null)
                    .Select(m => MethodInfoToControllerMethod(m))
                    .ToList();
            });
        }
        #endregion

        #region Controller_PropertyChanged
        /// <summary>
        /// Handles the PropertyChanged event of the Controller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SyncPropertyValue)
                PropertyChanges.Add(e.PropertyName);
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
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [Exclude]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
