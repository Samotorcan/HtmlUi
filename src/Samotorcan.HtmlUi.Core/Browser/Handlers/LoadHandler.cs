using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Browser.Handlers
{
    /// <summary>
    /// Load handler.
    /// </summary>
    [CLSCompliant(false)]
    public class LoadHandler : CefLoadHandler
    {
        #region Properties
        #region Protected

        #region OnLoadStart
        /// <summary>
        /// Called when the browser begins loading a frame. The |frame| value will
        /// never be empty -- call the IsMain() method to check if this frame is the
        /// main frame. Multiple frames may be loading at the same time. Sub-frames may
        /// start or continue loading after the main frame load has ended. This method
        /// may not be called for a particular frame if the load request for that frame
        /// fails. For notification of overall browser load status use
        /// OnLoadingStateChange instead.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        protected override void OnLoadStart(CefBrowser browser, CefFrame frame)
        {
            if (frame.IsMain)
            {
                BaseMainApplication.Current.InvokeOnMain(() =>
                {
                    BaseMainApplication.Current.Window.DestroyControllers();
                });
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
