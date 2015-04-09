using Samotorcan.HtmlUi.Core.Events;
using Samotorcan.HtmlUi.Core.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Renderer.Handlers
{
    /// <summary>
    /// LoadHandler
    /// </summary>
    internal class LoadHandler : CefLoadHandler
    {
        #region Events

        #region OnLoadEnd
        /// <summary>
        /// Occurs on load end.
        /// </summary>
        public event EventHandler<OnLoadEndEventArgs> OnLoadEndEvent;
        #endregion

        #endregion
        #region Methods
        #region Protected

        #region OnLoadEnd
        /// <summary>
        /// Called when the browser is done loading a frame. The |frame| value will
        /// never be empty -- call the IsMain() method to check if this frame is the
        /// main frame. Multiple frames may be loading at the same time. Sub-frames may
        /// start or continue loading after the main frame load has ended. This method
        /// will always be called for all frames irrespective of whether the request
        /// completes successfully.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="httpStatusCode"></param>
        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            if (OnLoadEndEvent != null)
                OnLoadEndEvent(this, new OnLoadEndEventArgs(browser, frame, httpStatusCode));
        }
        #endregion

        #endregion
        #endregion
    }
}
