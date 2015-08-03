using System;
using System.Linq;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Browser.Handlers
{
    /// <summary>
    /// Browser process handler.
    /// </summary>
    internal class ProcessHandler : CefBrowserProcessHandler
    {
        #region Methods
        #region Protected

        #region OnBeforeChildProcessLaunch
        /// <summary>
        /// Called before child process launch.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        protected override void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
        {
            if (commandLine == null)
                throw new ArgumentNullException("commandLine");

            if (!Application.Current.D3D11Enabled && !commandLine.GetArguments().Contains(Argument.DisableD3D11.Value))
                commandLine.AppendArgument(Argument.DisableD3D11.Value);
        }
        #endregion
        #region OnRenderProcessThreadCreated
        /// <summary>
        /// Called on the browser process IO thread after the main thread has been
        /// created for a new render process. Provides an opportunity to specify extra
        /// information that will be passed to
        /// CefRenderProcessHandler::OnRenderThreadCreated() in the render process. Do
        /// not keep a reference to |extra_info| outside of this method.
        /// </summary>
        /// <param name="extraInfo"></param>
        protected override void OnRenderProcessThreadCreated(CefListValue extraInfo)
        {
            if (extraInfo == null)
                throw new ArgumentNullException("extraInfo");

            extraInfo.SetString(0, Application.Current.NativeRequestUrl);
            extraInfo.SetString(1, Application.Current.RequestHostname);
            extraInfo.SetInt(2, Application.Current.NativeRequestPort);
        }
        #endregion

        #endregion
        #endregion
    }
}
