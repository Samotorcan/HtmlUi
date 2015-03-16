using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Exceptions;
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
    /// V8 native handler.
    /// </summary>
    internal class V8NativeHandler : CefV8Handler
    {
        #region Properties
        #region Public

        #region CefBrowser
        /// <summary>
        /// Gets or sets the cef browser.
        /// </summary>
        /// <value>
        /// The cef browser.
        /// </value>
        public CefBrowser CefBrowser { get; set; }
        #endregion

        #endregion
        #region Private

        #region Callbacks
        /// <summary>
        /// Gets or sets the callbacks.
        /// </summary>
        /// <value>
        /// The callbacks.
        /// </value>
        private Dictionary<Guid, JavascriptFunction> Callbacks { get; set; }
        #endregion
        #region Functions
        /// <summary>
        /// Gets or sets the functions.
        /// </summary>
        /// <value>
        /// The functions.
        /// </value>
        private Dictionary<string, JavascriptFunction> Functions { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="V8NativeHandler"/> class.
        /// </summary>
        public V8NativeHandler()
        {
            Callbacks = new Dictionary<Guid, JavascriptFunction>();
            Functions = new Dictionary<string, JavascriptFunction>();
        }

        #endregion
        #region Methods
        #region Public

        #region ProcessMessage
        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="sourceProcess">The source process.</param>
        /// <param name="processMessage">The process message.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "browser", Justification = "I want it to match to OnProcessMessageReceived method.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sourceProcess", Justification = "I want it to match to OnProcessMessageReceived method.")]
        public bool ProcessMessage(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            if (processMessage == null)
                throw new ArgumentNullException("processMessage");

            // native
            if (processMessage.Name == "native")
            {
                var message = MessageUtility.DeserializeMessage<string>(processMessage);

                if (message.CallbackId != null)
                {
                    var returnJson = message.Data;
                    var callback = GetCallback(message.CallbackId.Value);

                    if (callback != null)
                        callback.Execute(returnJson);
                }

                return true;
            }

            // call function
            else if (processMessage.Name == "callFunction")
            {
                var message = MessageUtility.DeserializeMessage(processMessage, new { Name = string.Empty, Data = new object() });
                var functionName = message.Data.Name;
                var data = message.Data.Data;

                if (Functions.ContainsKey(functionName))
                {
                    if (data != Undefined.Value)
                        Functions[functionName].Execute(data);
                    else
                        Functions[functionName].Execute();
                }
                else
                {
                    GeneralLog.Error(string.Format("Call function - function {0} is not registered.", functionName));
                }

                return true;
            }

            return false;
        }
        #endregion
        #region Reset
        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            Callbacks.Clear();
            Functions.Clear();
        }
        #endregion

        #endregion
        #region Protected

        #region Execute
        /// <summary>
        /// Sends the request to the browser process.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <param name="arguments"></param>
        /// <param name="returnValue"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            returnValue = null;
            exception = null;

            // native
            if (name == "native")
            {
                var functionName = arguments[0].GetStringValue();
                var jsonData = arguments[1].GetStringValue();
                var callbackFunction = arguments.Length > 2 && arguments[2].IsFunction ? arguments[2] : null;

                var callbackId = AddCallback(callbackFunction, CefV8Context.GetCurrentContext());

                MessageUtility.SendMessage(CefBrowser, "native", callbackId, new { Name = functionName, Json = jsonData });

                return true;
            }

            // register function
            else if (name == "registerFunction")
            {
                var functionName = arguments[0].GetStringValue();
                var function = arguments[1];

                if (!Functions.ContainsKey(functionName))
                    Functions.Add(functionName, new JavascriptFunction(function, CefV8Context.GetCurrentContext()));
                else
                    GeneralLog.Error(string.Format("Register function - function {0} is already registered.", functionName));
            }

            return false;
        }
        #endregion

        #endregion
        #region Private

        #region AddCallback
        /// <summary>
        /// Adds the callback.
        /// </summary>
        /// <param name="callbackFunction">The callback function.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private Guid? AddCallback(CefV8Value callbackFunction, CefV8Context context)
        {
            if (callbackFunction != null && callbackFunction.IsFunction)
            {
                var callback = new JavascriptFunction(callbackFunction, context);
                Callbacks.Add(callback.Id, callback);

                return callback.Id;
            }

            return null;
        }
        #endregion
        #region GetCallback
        /// <summary>
        /// Gets the callback.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private JavascriptFunction GetCallback(Guid id)
        {
            if (Callbacks.ContainsKey(id))
            {
                var callback = Callbacks[id];
                Callbacks.Remove(id);

                return callback;
            }

            return null;
        }
        #endregion

        #endregion
        #endregion
    }
}
