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
        private Dictionary<Guid, JavascriptCallback> Callbacks { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="V8NativeHandler"/> class.
        /// </summary>
        public V8NativeHandler()
        {
            Callbacks = new Dictionary<Guid, JavascriptCallback>();
        }

        #endregion
        #region Methods
        #region Public

        #region ProcessCallback
        /// <summary>
        /// Processes the callback.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="sourceProcess">The source process.</param>
        /// <param name="processMessage">The process message.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "browser", Justification = "I want it to match to OnProcessMessageReceived method.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sourceProcess", Justification = "I want it to match to OnProcessMessageReceived method.")]
        public bool ProcessCallback(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            if (processMessage.Name == "native")
            {
                var message = MessageUtility.DeserializeMessage<string>(processMessage);
                var returnJson = message.Data;
                var callback = GetCallback(message.CallbackId.Value);

                callback.Execute(returnJson);
                return true;
            }

            return false;
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

            if (name == "native")
            {
                var functionName = arguments[0].GetStringValue();
                var jsonData = arguments[1].GetStringValue();
                var callbackFunction = arguments[2];

                var callbackId = AddCallback(callbackFunction, CefV8Context.GetCurrentContext());

                MessageUtility.SendMessage(CefBrowser, functionName, callbackId, jsonData);

                return true;
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
                var callback = new JavascriptCallback(callbackFunction, context);
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
        private JavascriptCallback GetCallback(Guid id)
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
