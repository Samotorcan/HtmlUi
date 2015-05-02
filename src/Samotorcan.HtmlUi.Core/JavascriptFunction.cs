using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Javascript function.
    /// </summary>
    internal class JavascriptFunction
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
        public Guid Id { get; private set; }
        #endregion
        #region CallbackFunction
        /// <summary>
        /// Gets the callback function.
        /// </summary>
        /// <value>
        /// The callback function.
        /// </value>
        public CefV8Value CallbackFunction { get; private set; }
        #endregion
        #region Context
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public CefV8Context Context { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JavascriptFunction"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">
        /// function
        /// or
        /// context
        /// </exception>
        public JavascriptFunction(CefV8Value function, CefV8Context context)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (context == null)
                throw new ArgumentNullException("context");

            Id = Guid.NewGuid();
            CallbackFunction = function;
            Context = context;
        }

        #endregion
        #region Methods
        #region Public

        #region Execute
        /// <summary>
        /// Executes the function.
        /// </summary>
        public JToken Execute()
        {
            return ParseCefV8Value(CallbackFunction.ExecuteFunctionWithContext(Context, null, new CefV8Value[0]));
        }

        /// <summary>
        /// Executes the function with data.
        /// </summary>
        /// <param name="data">The data.</param>
        public JToken Execute(object data)
        {
            return ParseCefV8Value(CallbackFunction.ExecuteFunctionWithContext(Context, null, new CefV8Value[] { CefV8Value.CreateString(JsonUtility.SerializeToJson(data)) }));
        }

        /// <summary>
        /// Executes the function with json.
        /// </summary>
        /// <param name="json">The json.</param>
        public JToken Execute(string json)
        {
            return ParseCefV8Value(CallbackFunction.ExecuteFunctionWithContext(Context, null, new CefV8Value[] { CefV8Value.CreateString(json) }));
        }
        #endregion

        #endregion
        #region Private

        #region ParseCefV8Value
        /// <summary>
        /// Parses the cef v8 value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private JToken ParseCefV8Value(CefV8Value value)
        {
            if (value == null)
                return null;

            return CefUtility.RunInContext(Context, () =>
            {
                if (value.IsInt)
                    return JToken.FromObject(value.GetIntValue());

                if (value.IsUInt)
                    return JToken.FromObject(value.GetUIntValue());

                if (value.IsDouble)
                    return JToken.FromObject(value.GetDoubleValue());

                if (value.IsBool)
                    return JToken.FromObject(value.GetBoolValue());

                if (value.IsDate)
                    return JToken.FromObject(value.GetDateValue());

                if (value.IsString)
                    return JToken.FromObject(value.GetStringValue());

                if (value.IsUndefined)
                    return JValue.CreateUndefined();

                if (value.IsArray)
                {
                    var array = new JArray();

                    for (var i = 0; i < value.GetArrayLength(); i++)
                        array.Add(ParseCefV8Value(value.GetValue(i)));

                    return array;
                }

                if (value.IsObject)
                {
                    var obj = new JObject();

                    foreach (var propertyName in value.GetKeys())
                        obj.Add(propertyName, ParseCefV8Value(value.GetValue(propertyName)));

                    return obj;
                }

                return JValue.CreateNull();
            });
        }
        #endregion

        #endregion
        #endregion
    }
}
