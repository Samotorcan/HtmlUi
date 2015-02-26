using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Samotorcan.HtmlUi.Core.Handlers
{
    /// <summary>
    /// Default render process handler.
    /// </summary>
    [CLSCompliant(false)]
    public class DefaultRenderProcessHandler : CefRenderProcessHandler
    {
        #region Properties
        #region Private

        #region MessageRouter
        /// <summary>
        /// Gets or sets the message router.
        /// </summary>
        /// <value>
        /// The message router.
        /// </value>
        private CefMessageRouterRendererSide MessageRouter { get; set; }
        #endregion
        #region CefBrowser
        /// <summary>
        /// Gets or sets the cef browser.
        /// </summary>
        /// <value>
        /// The cef browser.
        /// </value>
        private CefBrowser CefBrowser { get; set; }
        #endregion
        #region NativeRequestUrl
        /// <summary>
        /// Gets or sets the native request URL.
        /// </summary>
        /// <value>
        /// The native request URL.
        /// </value>
        private string NativeRequestUrl { get; set; }
        #endregion
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
        /// Initializes a new instance of the <see cref="DefaultRenderProcessHandler"/> class.
        /// </summary>
        public DefaultRenderProcessHandler()
        {
            Callbacks = new Dictionary<Guid, JavascriptCallback>();
            MessageRouter = new CefMessageRouterRendererSide(new CefMessageRouterConfig());
        }

        #endregion
        #region Methods
        #region Protected

        #region OnContextCreated
        /// <summary>
        /// Called when context created.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="context">The context.</param>
        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            MessageRouter.OnContextCreated(browser, frame, context);
        }
        #endregion
        #region OnContextReleased
        /// <summary>
        /// Called when context released.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="context">The context.</param>
        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            MessageRouter.OnContextReleased(browser, frame, context);
        }
        #endregion
        #region OnProcessMessageReceived
        /// <summary>
        /// Called when process message received.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="sourceProcess">The source process.</param>
        /// <param name="processMessage">The process message.</param>
        /// <returns></returns>
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            if (processMessage == null)
                throw new ArgumentNullException("message");

            // digest callback
            if (processMessage.Name == "DigestCallback")
            {
                var message = MessageUtility.DeserializeMessage(processMessage);
                var callback = GetCallback(message.CallbackId.Value);

                callback.CallbackFunction.ExecuteFunctionWithContext(callback.Context, null, new CefV8Value[0]);
            }

            return MessageRouter.OnProcessMessageReceived(browser, sourceProcess, processMessage);
        }
        #endregion
        #region OnBrowserCreated
        /// <summary>
        /// Called after a browser has been created. When browsing cross-origin a new
        /// browser will be created before the old browser with the same identifier is
        /// destroyed.
        /// </summary>
        /// <param name="browser"></param>
        protected override void OnBrowserCreated(CefBrowser browser)
        {
            CefBrowser = browser;
        }
        #endregion
        #region OnBrowserDestroyed
        /// <summary>
        /// Called before a browser is destroyed.
        /// </summary>
        /// <param name="browser"></param>
        protected override void OnBrowserDestroyed(CefBrowser browser)
        {
            CefBrowser = null;
        }
        #endregion
        #region OnWebKitInitialized
        /// <summary>
        /// Called after WebKit has been initialized.
        /// </summary>
        protected override void OnWebKitInitialized()
        {
            // html-ui extension
            var htmlUiScript = ProcessExtensionResource(ResourceUtility.GetResourceAsString("Scripts/html-ui.extension.js"));

            CefRuntime.RegisterExtension("html-ui", htmlUiScript, new CefActionV8Handler("digest", Digest));
        }
        #endregion
        #region OnRenderThreadCreated
        /// <summary>
        /// Called after the render process main thread has been created.
        /// </summary>
        /// <param name="extraInfo"></param>
        protected override void OnRenderThreadCreated(CefListValue extraInfo)
        {
            log4net.GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            NativeRequestUrl = extraInfo.GetString(0);

            GeneralLog.Info("Render process thread created.");
        }
        #endregion

        #region OnUncaughtException
        /// <summary>
        /// Called for global uncaught exceptions in a frame. Execution of this
        /// callback is disabled by default. To enable set
        /// CefSettings.uncaught_exception_stack_size &gt; 0.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <param name="stackTrace"></param>
        protected override void OnUncaughtException(CefBrowser browser, CefFrame frame, CefV8Context context, CefV8Exception exception, CefV8StackTrace stackTrace)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            GeneralLog.Error("Render process unhandled exception: " + exception.Message);
        }
        #endregion

        #endregion
        #region Private

        #region Digest
        /// <summary>
        /// Digest call.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="obj">The object.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="returnValue">The return value.</param>
        /// <param name="exception">The exception.</param>
        private void Digest(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            exception = null;
            returnValue = null;

            var controllers = arguments[0];
            var callbackFunction = arguments.Length > 1 && arguments[1].IsFunction ? arguments[1] : null;
            var context = CefV8Context.GetCurrentContext();
            Guid? callbackId = null;

            // save callback
            if (callbackFunction != null)
                callbackId = AddCallback(callbackFunction, context);

            // controllers
            var controllerChanges = new List<ControllerChange>();

            for (int i = 0; i < controllers.GetArrayLength(); i++)
            {
                var controller = controllers.GetValue(i);

                var id = controller.GetValue("id").GetIntValue();
                var properties = controller.GetValue("properties");

                var controllerChange = new ControllerChange
                {
                    Id = id
                };
                controllerChanges.Add(controllerChange);

                // properties
                foreach (var propertyName in properties.GetKeys())
                {
                    var value = properties.GetValue(propertyName);
                    object rawValue = null;

                    if (value.IsString)
                        rawValue = value.GetStringValue();
                    else if (value.IsInt)
                        rawValue = value.GetIntValue();
                    else if (value.IsBool)
                        rawValue = value.GetBoolValue();
                    else if (value.IsDate)
                        rawValue = value.GetDateValue();
                    else if (value.IsDouble)
                        rawValue = value.GetDoubleValue();
                    else if (value.IsUInt)
                        rawValue = TryConvertToInt(value.GetUIntValue());

                    controllerChange.Properties.Add(propertyName, rawValue);
                }
            }

            // send to browser process
            MessageUtility.SendMessage(CefBrowser, "Digest", callbackId, controllerChanges);
        }
        #endregion
        #region ProcessExtensionResource
        /// <summary>
        /// Processes the extension resource.
        /// </summary>
        /// <param name="extensionResource">The extension resource.</param>
        /// <returns></returns>
        private string ProcessExtensionResource(string extensionResource)
        {
            var constants = new string[]
            {
                CreateStringConstant("nativeRequestUrl", NativeRequestUrl)
            };

            return extensionResource
                .Replace("// !native ", "native ")
                .Replace("// !inject-constants", string.Join(Environment.NewLine, constants));
        }
        #endregion
        #region CreateConstant
        /// <summary>
        /// Creates the constant.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string CreateConstant(string name, string value)
        {
            return string.Format("{0} = {1};", name, value);
        }
        #endregion
        #region CreateStringConstant
        /// <summary>
        /// Creates the string constant.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string CreateStringConstant(string name, string value)
        {
            return string.Format("{0} = '{1}';", name, value);
        }
        #endregion
        #region AddCallback
        /// <summary>
        /// Adds the callback.
        /// </summary>
        /// <param name="callbackFunction">The callback function.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private Guid AddCallback(CefV8Value callbackFunction, CefV8Context context)
        {
            var callback = new JavascriptCallback(callbackFunction, context);
            Callbacks.Add(callback.Id, callback);

            return callback.Id;
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
        #region TryConvertToInt
        /// <summary>
        /// Tries the convert to int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private object TryConvertToInt(uint value)
        {
            if (value > int.MaxValue)
                return value;

            return (int)value;
        }
        #endregion

        #region CurrentDomain_UnhandledException
        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GeneralLog.Error("Render process unhandled exception.", e.ExceptionObject as Exception);
        }
        #endregion

        #endregion
        #endregion
    }
}
