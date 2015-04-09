using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Samotorcan.HtmlUi.Core.Events;
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
        #region HtmlUiExtensions
        /// <summary>
        /// Gets or sets the HTML UI extensions.
        /// </summary>
        /// <value>
        /// The HTML UI extensions.
        /// </value>
        private List<string> HtmlUiExtensions { get; set; }
        #endregion
        #region LoadHandler
        /// <summary>
        /// Gets or sets the load handler.
        /// </summary>
        /// <value>
        /// The load handler.
        /// </value>
        private LoadHandler LoadHandler { get; set; }
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

            LoadHandler = new LoadHandler();
            LoadHandler.OnLoadEndEvent += OnLoadEnd;

            HtmlUiExtensions = GetHtmlUiExtensions();
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
            CefRuntime.RegisterExtension("html-ui.extension.native", GetHtmlUiExtensionScript("native", false), V8NativeHandler);

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
        #region OnLoadEnd
        /// <summary>
        /// Called when load end.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="OnLoadEndEventArgs"/> instance containing the event data.</param>
        protected void OnLoadEnd(object sender, OnLoadEndEventArgs e)
        {
            var frame = e.Frame;
            var context = e.Frame.V8Context;
            var browser = e.Browser;

            if (frame.IsMain && V8NativeHandler != null)
                V8NativeHandler.Reset();

            RegisterAngularDeferredBootstrap(context);
            RegisterHtmlUiAsScriptIfNeeded(context);
            RegisterHtmlUiInit(context);
        }
        #endregion

        #region GetLoadHandler
        /// <summary>
        /// Return the handler for browser load status events.
        /// </summary>
        /// <returns></returns>
        protected override CefLoadHandler GetLoadHandler()
        {
            return LoadHandler;
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
            foreach (var htmlUiExtension in HtmlUiExtensions)
                CefRuntime.RegisterExtension("html-ui.extension." + htmlUiExtension, GetHtmlUiExtensionScript(htmlUiExtension), V8NativeHandler);
#endif
        }
        #endregion
        #region RegisterHtmlUiAsScriptIfNeeded
        /// <summary>
        /// Registers the HTML UI as script if needed.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "context", Justification = "Debug only.")]
        private void RegisterHtmlUiAsScriptIfNeeded(CefV8Context context)
        {
#if DEBUG
            foreach (var htmlUiExtension in HtmlUiExtensions)
                EvalHtmlUiExtensionScript(htmlUiExtension, context);
#endif
        }
        #endregion
        #region GetHtmlUiExtensionScript
        /// <summary>
        /// Gets the HTML UI extension script.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="mapping">if set to <c>true</c> include mapping.</param>
        /// <returns></returns>
        private string GetHtmlUiExtensionScript(string name, bool mapping)
        {
            var scriptName = string.Format("Scripts/html-ui.extension.{0}.js", name);
            var script = ResourceUtility.GetResourceAsString(scriptName);

            var constants = new string[]
            {
                CreateStringConstant("_nativeRequestUrl", NativeRequestUrl)
            };

            var processedExtensionResource = script
                .Replace("// !native ", "native ")
                .Replace("// !inject-constants", string.Join(Environment.NewLine, constants));

            processedExtensionResource = Regex.Replace(processedExtensionResource, @"^/// <reference path=.*$", string.Empty, RegexOptions.Multiline);

            if (mapping)
            {
                var typescriptName = string.Format("Scripts/html-ui.extension.{0}.ts", name);
                var typescript = ResourceUtility.GetResourceAsString(typescriptName);

                processedExtensionResource = Regex.Replace(processedExtensionResource, @"^//# sourceMappingURL=.*$", (match) =>
                {
                    var sourceMappingURL = match.Groups[0].Value.Substring("//# sourceMappingURL=".Length);

                    var map = JsonConvert.DeserializeObject<SourceMap>(ResourceUtility.GetResourceAsString(string.Format("Scripts/{0}", sourceMappingURL)));
                    map.SourcesContent = new List<string> { typescript };
                    map.File = null;
                    map.SourceRoot = "/Scripts";

                    return string.Format("//# sourceMappingURL=data:application/octet-stream;base64,{0}",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonUtility.SerializeToJson(map))));
                }, RegexOptions.Multiline);
            }
            else
            {
                processedExtensionResource = Regex.Replace(processedExtensionResource, @"^//# sourceMappingURL=.*$", string.Empty, RegexOptions.Multiline);
            }

            return processedExtensionResource;
        }

        /// <summary>
        /// Gets the HTML UI extension script.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string GetHtmlUiExtensionScript(string name)
        {
            return GetHtmlUiExtensionScript(name, true);
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

            if (!context.TryEval(GetHtmlUiExtensionScript(name, true), out returnValue, out exception))
                GeneralLog.Error(string.Format("Register html ui script exception: {0}.", JsonConvert.SerializeObject(exception)));
        }
        #endregion
        #region GetHtmlUiExtensions
        /// <summary>
        /// Gets the HTML UI extensions.
        /// </summary>
        /// <returns></returns>
        private List<string> GetHtmlUiExtensions()
        {
            var htmlUiExtensions = new List<string>();

            foreach (var resourceName in ResourceUtility.GetResourceNames())
            {
                var match = Regex.Match(resourceName, @"^Scripts\.html-ui\.extension\.([a-zA-Z0-9\-_]*)\.ts$");
                var htmlUiExtension = match.Groups[1].Value;

                if (match.Success && htmlUiExtension != "native")
                    htmlUiExtensions.Add(htmlUiExtension);
            }

            return htmlUiExtensions;
        }
        #endregion
        #region RegisterAngularDeferredBootstrap
        /// <summary>
        /// Registers the angular deferred bootstrap.
        /// </summary>
        /// <param name="context">The context.</param>
        private void RegisterAngularDeferredBootstrap(CefV8Context context)
        {
            CefV8Value returnValue = null;
            CefV8Exception exception = null;

            var code = "window.name = 'NG_DEFER_BOOTSTRAP!'";

            if (!context.TryEval(code, out returnValue, out exception))
                GeneralLog.Error(string.Format("Register angular deferred bootstrap exception: {0}.", JsonConvert.SerializeObject(exception)));
        }
        #endregion
        #region RegisterHtmlUiInit
        /// <summary>
        /// Registers the HTML UI initialize.
        /// </summary>
        /// <param name="context">The context.</param>
        private void RegisterHtmlUiInit(CefV8Context context)
        {
            CefV8Value returnValue = null;
            CefV8Exception exception = null;

            var code = "htmlUi.init()";

            if (!context.TryEval(code, out returnValue, out exception))
                GeneralLog.Error(string.Format("Register html ui init exception: {0}.", JsonConvert.SerializeObject(exception)));
        }
        #endregion

        #endregion
        #endregion
    }
}
