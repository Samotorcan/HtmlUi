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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Samotorcan.HtmlUi.Core.Renderer.Handlers
{
    /// <summary>
    /// Render process handler.
    /// </summary>
    internal class ProcessHandler : CefRenderProcessHandler
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
        #region V8NativeHandler
        /// <summary>
        /// Gets or sets the v8 native handler.
        /// </summary>
        /// <value>
        /// The v8 native handler.
        /// </value>
        private V8NativeHandler V8NativeHandler { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessHandler"/> class.
        /// </summary>
        public ProcessHandler()
        {
            MessageRouter = new CefMessageRouterRendererSide(new CefMessageRouterConfig());
            V8NativeHandler = new V8NativeHandler();
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
            if (frame == null)
                throw new ArgumentNullException("frame");

            if (frame.IsMain && V8NativeHandler != null)
                V8NativeHandler.Reset();

            RegisterHtmlUiAsScriptIfNeeded(context);

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
        /// <param name="message">The process message.</param>
        /// <returns></returns>
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            return V8NativeHandler.ProcessMessage(browser, sourceProcess, message) ||
                MessageRouter.OnProcessMessageReceived(browser, sourceProcess, message);
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
            V8NativeHandler.CefBrowser = browser;
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
            var htmlUiNativeScript = ProcessExtensionResource(ResourceUtility.GetResourceAsString("Scripts/html-ui.extension.native.js"));

            CefRuntime.RegisterExtension("html-ui-native", htmlUiNativeScript, V8NativeHandler);

            RegisterHtmlUiAsExtensionIfNeeded();
        }
        #endregion
        #region OnRenderThreadCreated
        /// <summary>
        /// Called after the render process main thread has been created.
        /// </summary>
        /// <param name="extraInfo"></param>
        protected override void OnRenderThreadCreated(CefListValue extraInfo)
        {
            if (extraInfo == null)
                throw new ArgumentNullException("extraInfo");

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

        #region ProcessExtensionResource
        /// <summary>
        /// Processes the extension resources.
        /// </summary>
        /// <param name="extensionResources">The extension resources.</param>
        /// <returns></returns>
        private string ProcessExtensionResource(params string[] extensionResources)
        {
            var constants = new string[]
            {
                CreateStringConstant("_nativeRequestUrl", NativeRequestUrl)
            };

            var processedExtensionResources = new List<string>();

            foreach (var extensionResource in extensionResources)
            {
                var processedExtensionResource = extensionResource
                    .Replace("// !native ", "native ")
                    .Replace("// !inject-constants", string.Join(Environment.NewLine, constants));

                processedExtensionResource = Regex.Replace(processedExtensionResource, @"^/// <reference path=.*$", string.Empty, RegexOptions.Multiline);
                processedExtensionResource = Regex.Replace(processedExtensionResource, @"^//# sourceMappingURL=.*$", string.Empty, RegexOptions.Multiline);

                processedExtensionResources.Add(processedExtensionResource);
            }

            return string.Join(Environment.NewLine, processedExtensionResources);
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
        
        #region RegisterHtmlUiAsExtensionIfNeeded
        /// <summary>
        /// Registers the HTML UI as extension if needed.
        /// </summary>
        private void RegisterHtmlUiAsExtensionIfNeeded()
        {
#if !DEBUG
            CefRuntime.RegisterExtension("html-ui.extension.main", GetHtmlUiExtensionScript("main"), V8NativeHandler);
            CefRuntime.RegisterExtension("html-ui.extension.services", GetHtmlUiExtensionScript("services"), V8NativeHandler);
            CefRuntime.RegisterExtension("html-ui.extension.settings", GetHtmlUiExtensionScript("settings"), V8NativeHandler);
#endif
        }
        #endregion
        #region RegisterHtmlUiAsScriptIfNeeded
        /// <summary>
        /// Registers the HTML UI as script if needed.
        /// </summary>
        private void RegisterHtmlUiAsScriptIfNeeded(CefV8Context context)
        {
#if DEBUG
            EvalHtmlUiExtensionScript("main", context);
            EvalHtmlUiExtensionScript("services", context);
            EvalHtmlUiExtensionScript("settings", context);
#endif
        }
        #endregion
        #region GetHtmlUiExtensionScript
        /// <summary>
        /// Gets the HTML UI extension script.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string GetHtmlUiExtensionScript(string name)
        {
            return ProcessExtensionResource(ResourceUtility.GetResourceAsString(string.Format("Scripts/html-ui.extension.{0}.js", name))) +
                Environment.NewLine +
                string.Format("//# sourceURL=html-ui.extension.{0}.js", name);
        }
        #endregion
        #region EvalHtmlUiExtensionScript
        /// <summary>
        /// Evals the HTML UI extension script.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        private void EvalHtmlUiExtensionScript(string name, CefV8Context context)
        {
            CefV8Value returnValue = null;
            CefV8Exception exception = null;

            if (!context.TryEval(GetHtmlUiExtensionScript(name), out returnValue, out exception))
                GeneralLog.Error(string.Format("Register html ui script exception: {0}.", JsonConvert.SerializeObject(exception)));
        }
        #endregion

        #endregion
        #endregion
    }
}
