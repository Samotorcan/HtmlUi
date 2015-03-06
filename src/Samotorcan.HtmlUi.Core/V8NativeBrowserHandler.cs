using Newtonsoft.Json;
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
    /// V8 native browser handler.
    /// </summary>
    internal class V8NativeBrowserHandler
    {
        #region Properties
        #region Private

        #region ProcessMessages
        /// <summary>
        /// Gets or sets the process messages.
        /// </summary>
        /// <value>
        /// The process messages.
        /// </value>
        private Dictionary<string, Func<string, object>> ProcessMessages { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="V8NativeBrowserHandler"/> class.
        /// </summary>
        /// <param name="processMessages">The process messages.</param>
        public V8NativeBrowserHandler(Dictionary<string, Func<string, object>> processMessages)
        {
            if (processMessages == null)
                processMessages = new Dictionary<string, Func<string, object>>();

            ProcessMessages = processMessages;
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
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sourceProcess", Justification = "I want it to match to OnProcessMessageReceived method.")]
        public bool ProcessMessage(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var name = processMessage.Name;

            if (ProcessMessages.ContainsKey(name))
            {
                var message = MessageUtility.DeserializeMessage<string>(processMessage);

                BaseMainApplication.Current.InvokeOnMainAsync(() =>
                {
                    var returnData = ProcessMessages[name](message.Data);
                    var returnJson = JsonConvert.SerializeObject(returnData);

                    MessageUtility.SendMessage(browser, "native", message.CallbackId, returnJson);
                }).ContinueWith(t =>
                {
                    GeneralLog.Error("Native call exception.", t.Exception);
                }, TaskContinuationOptions.OnlyOnFaulted);

                return true;
            }

            return false;
        }
        #endregion

        #endregion
        #endregion
    }
}
