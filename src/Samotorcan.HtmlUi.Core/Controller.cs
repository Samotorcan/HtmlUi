using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Attributes;
using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors", Justification = "All classes that extend Controller must have this exact constructor.")]
    public abstract class Controller : IDisposable
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

        #region ControllerTypeInfo
        /// <summary>
        /// Gets or sets the controller type information.
        /// </summary>
        /// <value>
        /// The controller type information.
        /// </value>
        internal ControllerTypeInfo ControllerTypeInfo { get; set; }
        #endregion
        #region ControllerTypeInfos
        /// <summary>
        /// Gets or sets the controller type informations.
        /// </summary>
        /// <value>
        /// The controller type informations.
        /// </value>
        internal static Dictionary<Type, ControllerTypeInfo> ControllerTypeInfos { get; set; }
        #endregion

        #endregion
        #region Protected

        #region ControllerType
        /// <summary>
        /// Gets or sets the type of the controller.
        /// </summary>
        /// <value>
        /// The type of the controller.
        /// </value>
        protected Type ControllerType { get; set; }
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
        public Controller() { }

        #endregion
        #region Methods
        #region Public

        #region CallFunction
        /// <summary>
        /// Calls the function asynchronous.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        [Exclude]
        public Task<TResult> CallFunctionAsync<TResult>(string name, params object[] arguments)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(name);

            if (arguments == null)
                arguments = new object[0];

            Application.Current.EnsureMainThread();

            var resultAction = Application.Current.Window.CallFunctionAsync("callClientFunction", new ClientFunction { ControllerId = Id, Name = name, Arguments = arguments });

            return resultAction.ContinueWith<TResult>((task) =>
            {
                var result = task.Result.ToObject<ClientFunctionResult>(JsonUtility.Serializer);

                if (result.Type == ClientFunctionResultType.Exception)
                    throw new CallFunctionException(result.Exception);

                if (result.Type == ClientFunctionResultType.FunctionNotFound)
                    throw new FunctionNotFoundException(name);

                if (result.Type == ClientFunctionResultType.Undefined)
                    return default(TResult);

                return result.Value.ToObject<TResult>();
            });
        }

        /// <summary>
        /// Calls the function asynchronous.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [Exclude]
        public Task<TResult> CallFunctionAsync<TResult>(string name)
        {
            return CallFunctionAsync<TResult>(name, null);
        }

        /// <summary>
        /// Calls the function asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        [Exclude]
        public Task CallFunctionAsync(string name, params object[] arguments)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(name);

            if (arguments == null)
                arguments = new object[0];

            Application.Current.EnsureMainThread();

            return Application.Current.Window.CallFunctionAsync("callClientFunction", new ClientFunction { ControllerId = Id, Name = name, Arguments = arguments });
        }

        /// <summary>
        /// Calls the function asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [Exclude]
        public Task CallFunctionAsync(string name)
        {
            return CallFunctionAsync(name, null);
        }
        #endregion

        #endregion
        #region Internal

        #region Initialize
        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="id">The identifier.</param>
        internal virtual void Initialize(int id)
        {
            Id = id;

            ControllerType = GetType();
            Name = ControllerType.Name;

            LoadControllerTypeInfoIfNeeded();
            ControllerTypeInfo = ControllerTypeInfos[ControllerType];
        }
        #endregion
        #region WarmUp
        /// <summary>
        /// Warms up the native calls.
        /// </summary>
        /// <param name="warmUp">The warm up.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "warmUp", Justification = "Warm up method.")]
        [InternalMethod]
        internal object WarmUp(string warmUp)
        {
            return new object();
        }
        #endregion
        #region GetMethods
        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <returns></returns>
        internal List<ControllerMethodDescription> GetMethods()
        {
            return ControllerTypeInfo.Methods.Values.Select(m => new ControllerMethodDescription
            {
                Name = m.Name
            })
            .ToList();
        }
        #endregion
        #region GetControllerDescription
        /// <summary>
        /// Gets the controller description.
        /// </summary>
        /// <returns></returns>
        internal ControllerDescription GetControllerDescription()
        {
            return new ControllerDescription
            {
                Id = Id,
                Name = Name,
                Methods = GetMethods()
            };
        }
        #endregion
        
        #region CallMethod
        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="internalMethod">if set to <c>true</c> [internal method].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        /// <exception cref="MethodNotFoundException"></exception>
        /// <exception cref="ParameterCountMismatchException"></exception>
        internal object CallMethod(string methodName, JArray arguments, bool internalMethod)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException("methodName");

            if (arguments == null)
                arguments = new JArray();

            var method = FindMethod(methodName, internalMethod);

            if (method == null)
                throw new MethodNotFoundException(methodName, Name);

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

                return Value.Undefined;
            }
            else
            {
                return method.Delegate.DynamicInvoke(delegateParameters.ToArray());
            }
        }

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        /// <exception cref="MethodNotFoundException"></exception>
        /// <exception cref="ParameterCountMismatchException"></exception>
        internal object CallMethod(string methodName, JArray arguments)
        {
            return CallMethod(methodName, arguments, false);
        }
        #endregion

        #endregion
        #region Private

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
                Logger.Warn(string.Format("Overloaded methods are not supported. (controller = \"{0}\", method = \"{1}\")", Name, methodInfo.Name));
                isValid = false;
            }
            else if (methodInfo.GetParameters().Any(p => p.ParameterType.IsByRef))
            {
                Logger.Warn(string.Format("Ref parameters are not supported. (controller = \"{0}\", method = \"{1}\")", Name, methodInfo.Name));
                isValid = false;
            }
            else if (methodInfo.GetParameters().Any(p => p.IsOut))
            {
                Logger.Warn(string.Format("Out parameters are not supported. (controller = \"{0}\", method = \"{1}\")", Name, methodInfo.Name));
                isValid = false;
            }
            else if (methodInfo.IsGenericMethod)
            {
                Logger.Warn(string.Format("Generic methods are not supported. (controller = \"{0}\", method = \"{1}\")", Name, methodInfo.Name));
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
        #region FindMethods
        /// <summary>
        /// Finds the methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private Dictionary<string, ControllerMethod> FindMethods(Type type)
        {
            var methods = new Dictionary<string, ControllerMethod>();

            while (type != typeof(object))
            {
                var methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName && m.GetCustomAttribute<ExcludeAttribute>() == null)
                    .ToList();

                foreach (var methodInfo in methodInfos)
                {
                    if (IsValidMethod(methodInfo, methodInfos))
                    {
                        var controllerMethod = MethodInfoToControllerMethod(methodInfo);
                        methods.Add(controllerMethod.Name, controllerMethod);
                    }
                }

                type = type.BaseType;
            }

            return methods;
        }
        #endregion
        #region FindInternalMethods
        /// <summary>
        /// Finds the internal methods.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, ControllerMethod> FindInternalMethods()
        {
            return typeof(Controller).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName && m.GetCustomAttribute<InternalMethodAttribute>() != null)
                .Select(m => MethodInfoToControllerMethod(m))
                .ToDictionary(m => m.Name, m => m);
        }
        #endregion
        #region FindMethod
        /// <summary>
        /// Finds the method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="internalMethod">if set to <c>true</c> [internal method].</param>
        /// <returns></returns>
        private ControllerMethod FindMethod(string methodName, bool internalMethod)
        {
            var methods = internalMethod
                ? ControllerTypeInfo.InternalMethods
                : ControllerTypeInfo.Methods;

            ControllerMethod method = null;

            if (methods.TryGetValue(methodName, out method))
                return method;

            if (methods.TryGetValue(StringUtility.PascalCase(methodName), out method))
                return method;

            if (methods.TryGetValue(StringUtility.CamelCase(methodName), out method))
                return method;

            return null;
        }

        /// <summary>
        /// Finds the method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        private ControllerMethod FindMethod(string methodName)
        {
            return FindMethod(methodName, false);
        }
        #endregion
        #region LoadControllerTypeInfoIfNeeded
        /// <summary>
        /// Loads the controller type information if needed.
        /// </summary>
        private void LoadControllerTypeInfoIfNeeded()
        {
            if (!ControllerTypeInfos.ContainsKey(ControllerType))
            {
                ControllerTypeInfos.Add(ControllerType, new ControllerTypeInfo
                {
                    Methods = FindMethods(ControllerType),
                    InternalMethods = FindInternalMethods()
                });
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
    }
}
