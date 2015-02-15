using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Handlers.Browser
{
    /// <summary>
    /// Default life span handler.
    /// </summary>
    [CLSCompliant(false)]
    public class DefaultCefLifeSpanHandler : CefLifeSpanHandler
    {
        #region Events

        #region BrowserCreated
        /// <summary>
        /// Occurs when the browser is created.
        /// </summary>
        public event EventHandler<CefBrowser> BrowserCreated;
        #endregion

        #endregion
        #region Methods
        #region Protected

        #region OnBeforeClose
        /// <summary>
        /// Called just before a browser is destroyed. Release all references to the
        /// browser object and do not attempt to execute any methods on the browser
        /// object after this callback returns. If this is a modal window and a custom
        /// modal loop implementation was provided in RunModal() this callback should
        /// be used to exit the custom modal loop. See DoClose() documentation for
        /// additional usage information.
        /// </summary>
        /// <param name="browser"></param>
        protected override void OnBeforeClose(CefBrowser browser)
        {
            Application.Current.BrowserMessageRouter.OnBeforeClose(browser);
        }
        #endregion
        #region OnAfterCreated
        /// <summary>
        /// Called after a new browser is created.
        /// </summary>
        /// <param name="browser"></param>
        protected override void OnAfterCreated(CefBrowser browser)
        {
            if (BrowserCreated != null)
                BrowserCreated(this, browser);
        }
        #endregion

        #endregion
        #endregion
    }
}
