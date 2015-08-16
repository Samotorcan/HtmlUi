using Samotorcan.HtmlUi.Core.Renderer.Handlers;
using System;
using Xilium.CefGlue;
using Samotorcan.HtmlUi.Core.Events;

namespace Samotorcan.HtmlUi.Core.Renderer
{
    /// <summary>
    /// Renderer app.
    /// </summary>
    internal class App : CefApp
    {
        public event EventHandler<BrowserCreatedEventArgs> BrowserCreated;

        #region Properties
        #region Private

        #region RenderProcessHandler
        /// <summary>
        /// Gets or sets the render process handler.
        /// </summary>
        /// <value>
        /// The render process handler.
        /// </value>
        private ProcessHandler RenderProcessHandler { get; set; }
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
            RenderProcessHandler = new ProcessHandler();
            RenderProcessHandler.BrowserCreated += (s, e) =>
            {
                if (BrowserCreated != null)
                    BrowserCreated(this, new BrowserCreatedEventArgs(e.CefBrowser));
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
