using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Handlers
{
    /// <summary>
    /// Default CEF app.
    /// </summary>
    [CLSCompliant(false)]
    public class DefaultCefApp : CefApp
    {
        #region Properties
        #region Private

        #region BrowserProcessHandler
        /// <summary>
        /// Gets or sets the browser process handler.
        /// </summary>
        /// <value>
        /// The browser process handler.
        /// </value>
        private DefaultBrowserProcessHandler BrowserProcessHandler { get; set; }
        #endregion
        #region RenderProcessHandler
        /// <summary>
        /// Gets or sets the render process handler.
        /// </summary>
        /// <value>
        /// The render process handler.
        /// </value>
        private DefaultRenderProcessHandler RenderProcessHandler { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCefApp"/> class.
        /// </summary>
        public DefaultCefApp()
            : base()
        {
            BrowserProcessHandler = new DefaultBrowserProcessHandler();
            RenderProcessHandler = new DefaultRenderProcessHandler();
        }

        #endregion
        #region Methods
        #region Protected

        #region OnBeforeCommandLineProcessing
        /// <summary>
        /// Called before command line processing.
        /// </summary>
        /// <param name="processType">Type of the process.</param>
        /// <param name="commandLine">The command line.</param>
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            // TODO: currently on linux platform location of locales and pack files are determined
            // incorrectly (relative to main module instead of libcef.so module).
            // Once issue http://code.google.com/p/chromiumembedded/issues/detail?id=668 will be resolved
            // this code can be removed.
            if (CefRuntime.Platform == CefRuntimePlatform.Linux)
            {
                var path = PathUtility.Application;
                path = Path.GetDirectoryName(path);

                commandLine.AppendSwitch("resources-dir-path", path);
                commandLine.AppendSwitch("locales-dir-path", Path.Combine(path, "locales"));
            }

            if (!Application.Current.EnableD3D11)
                commandLine.AppendArgument(CefArgument.DisableD3D11.Value);
        }
        #endregion
        #region GetBrowserProcessHandler
        /// <summary>
        /// Gets the browser process handler.
        /// </summary>
        /// <returns></returns>
        protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        {
            return BrowserProcessHandler;
        }
        #endregion
        #region GetRenderProcessHandler
        /// <summary>
        /// Gets the render process handler.
        /// </summary>
        /// <returns></returns>
        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return RenderProcessHandler;
        }
        #endregion

        #endregion
        #endregion
    }
}
