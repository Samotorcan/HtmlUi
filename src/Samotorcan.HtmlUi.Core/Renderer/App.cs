using Samotorcan.HtmlUi.Core.Renderer.Handlers;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Renderer
{
    /// <summary>
    /// Renderer app.
    /// </summary>
    [CLSCompliant(false)]
    public class App : CefApp
    {
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
