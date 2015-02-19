using Samotorcan.HtmlUi.Core.Utilities;
using Samotorcan.HtmlUi.Core.Validation;
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
            Argument.Null(commandLine, "commandLine");

            commandLine.AppendSwitch("resources-dir-path", PathUtility.WorkingDirectory);
            commandLine.AppendSwitch("locales-dir-path", Path.Combine(PathUtility.WorkingDirectory, "locales"));

            if (!BaseApplication.Current.EnableD3D11)
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
