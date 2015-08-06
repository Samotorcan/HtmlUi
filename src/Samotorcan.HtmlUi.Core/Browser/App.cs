using Samotorcan.HtmlUi.Core.Browser.Handlers;
using System;
using Xilium.CefGlue;
using Samotorcan.HtmlUi.Core.Events;

namespace Samotorcan.HtmlUi.Core.Browser
{
    /// <summary>
    /// Browser app.
    /// </summary>
    internal class App : CefApp
    {
        #region Events

        #region ContextInitialized
        /// <summary>
        /// Occurs when context is initialized.
        /// </summary>
        public event EventHandler<ContextInitializedEventArgs> ContextInitialized;
        #endregion

        #endregion
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

            BrowserProcessHandler.ContextInitialized += (s, args) =>
            {
                if (ContextInitialized != null)
                    ContextInitialized(this, new ContextInitializedEventArgs());
            };
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
