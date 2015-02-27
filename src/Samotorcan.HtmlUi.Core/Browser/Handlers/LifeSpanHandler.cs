using Samotorcan.HtmlUi.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Browser.Handlers
{
    /// <summary>
    /// Life span handler.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "LifeSpan", Justification = "The base class has the same name.")]
    [CLSCompliant(false)]
    public class LifeSpanHandler : CefLifeSpanHandler
    {
        #region Events

        #region BrowserCreated
        /// <summary>
        /// Occurs when the browser is created.
        /// </summary>
        public event EventHandler<BrowserCreatedEventArgs> BrowserCreated;
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
            BaseMainApplication.Current.BrowserMessageRouter.OnBeforeClose(browser);
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
                BrowserCreated(this, new BrowserCreatedEventArgs(browser));
        }
        #endregion

        #endregion
        #endregion
    }
}
