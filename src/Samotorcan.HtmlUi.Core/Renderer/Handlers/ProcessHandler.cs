using Newtonsoft.Json;
using Samotorcan.HtmlUi.Core.Events;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Renderer.Handlers
{
    /// <summary>
    /// Render process handler.
    /// </summary>
    internal class ProcessHandler : CefRenderProcessHandler
    {
        public event EventHandler<BrowserCreatedEventArgs> BrowserCreated;

        #region Properties
        #region Private

        #region NativeMessageHandler
        /// <summary>
        /// Gets or sets the native message handler.
        /// </summary>
        /// <value>
        /// The native message handler.
        /// </value>
        private NativeMessageHandler NativeMessageHandler { get; set; }
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
        private ChildApplication Application
        {
            get
            {
                return ChildApplication.Current;
            }
        }

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessHandler"/> class.
        /// </summary>
        public ProcessHandler()
        {
            NativeMessageHandler = new NativeMessageHandler();

            LoadHandler = new LoadHandler();
            LoadHandler.OnLoadStartEvent += OnLoadStart;
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

            return NativeMessageHandler.ProcessMessage(browser, sourceProcess, message);
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
            NativeMessageHandler.CefBrowser = browser;

            if (BrowserCreated != null)
                BrowserCreated(this, new BrowserCreatedEventArgs(browser));
        }
        #endregion
        #region OnBrowserDestroyed
        /// <summary>
        /// Called before a browser is destroyed.
        /// </summary>
        /// <param name="browser"></param>
        protected override void OnBrowserDestroyed(CefBrowser browser)
        {
            
        }
        #endregion
        #region OnWebKitInitialized
        /// <summary>
        /// Called after WebKit has been initialized.
        /// </summary>
        protected override void OnWebKitInitialized()
        {
            Logger.Info("WebKit initialized.");

            CefRuntime.RegisterExtension("htmlUi.native", GetHtmlUiScript("native", false), NativeMessageHandler);

            if (!Application.IncludeHtmUiScriptMapping)
                CefRuntime.RegisterExtension("htmlUi.main", GetHtmlUiScript("main", false), NativeMessageHandler);
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

            var syncProperties = JsonConvert.DeserializeObject<Dictionary<string, object>>(extraInfo.GetString(0));

            foreach (var propertyName in syncProperties.Keys)
                Application.SetSyncProperty(propertyName, syncProperties[propertyName]);

            Logger.Info("Render process thread created.");
        }
        #endregion
        #region OnLoadStart
        /// <summary>
        /// Called when load start.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="OnLoadStartEventArgs"/> instance containing the event data.</param>
        protected void OnLoadStart(object sender, OnLoadStartEventArgs e)
        {
            var frame = e.Frame;
            var context = e.Frame.V8Context;

            if (frame.IsMain && IsLocalUrl(frame.Url))
            {
                if (NativeMessageHandler != null)
                    NativeMessageHandler.Reset();

                if (Application.IncludeHtmUiScriptMapping)
                    EvalHtmlUiScript("main", context);

                EvalHtmlUiScript("run", context, false);
            }
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

            Logger.Error(string.Format("Unhandled exception: {0}", JsonConvert.SerializeObject(exception)));
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
        #region GetHtmlUiScript
        /// <summary>
        /// Gets the HTML UI script.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="mapping">if set to <c>true</c> include mapping.</param>
        /// <returns></returns>
        private string GetHtmlUiScript(string name, bool mapping)
        {
            var scriptName = string.Format("Scripts/htmlUi.{0}.js", name);
            var script = ResourceUtility.GetResourceAsString(scriptName);

            var constants = new string[]
            {
                CreateStringConstant("_nativeRequestUrl", Application.NativeRequestUrl)
            };

            var processedExtensionResource = script
                .Replace("// !native ", "native ")
                .Replace("// !inject-constants", string.Join(Environment.NewLine, constants));

            processedExtensionResource = Regex.Replace(processedExtensionResource, @"^/// <reference path=.*$", string.Empty, RegexOptions.Multiline);

            if (mapping)
            {
                var typescriptName = string.Format("TypeScript/htmlUi.{0}.ts", name);
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
        /// Gets the HTML UI script.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string GetHtmlUiScript(string name)
        {
            return GetHtmlUiScript(name, true);
        }
        #endregion
        #region EvalHtmlUiScript
        /// <summary>
        /// Evals the HTML UI script.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        private void EvalHtmlUiScript(string name, CefV8Context context, bool mapping)
        {
            CefV8Value returnValue = null;
            CefV8Exception exception = null;

            if (!context.TryEval(GetHtmlUiScript(name, mapping), out returnValue, out exception))
                Logger.Error(string.Format("Register html ui script exception: {0}.", JsonConvert.SerializeObject(exception)));
        }

        /// <summary>
        /// Evals the HTML UI script.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        private void EvalHtmlUiScript(string name, CefV8Context context)
        {
            EvalHtmlUiScript(name, context, true);
        }
        #endregion
        #region IsLocalUrl
        /// <summary>
        /// Determines whether the specified URL is local.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        internal bool IsLocalUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            return UrlUtility.IsLocalUrl(Application.RequestHostname, Application.NativeRequestPort, url);
        }
        #endregion

        #endregion
        #endregion
    }
}
