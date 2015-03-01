using Samotorcan.HtmlUi.Core.Browser.Handlers;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Browser
{
    /// <summary>
    /// Browser app.
    /// </summary>
    [CLSCompliant(false)]
    public class App : CefApp
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
        private ProcessHandler BrowserProcessHandler { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
            : base()
        {
            BrowserProcessHandler = new ProcessHandler();
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
            if (commandLine == null)
                throw new ArgumentNullException("commandLine");

            if (!commandLine.HasSwitch("resources-dir-path"))
                commandLine.AppendSwitch("resources-dir-path", PathUtility.WorkingDirectory);

            if (!commandLine.HasSwitch("locales-dir-path"))
                commandLine.AppendSwitch("locales-dir-path", Path.Combine(PathUtility.WorkingDirectory, "locales"));

            if (!BaseMainApplication.Current.EnableD3D11 && !commandLine.GetArguments().Contains(Argument.DisableD3D11.Value))
                commandLine.AppendArgument(Argument.DisableD3D11.Value);
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

        #endregion
        #endregion
    }
}
