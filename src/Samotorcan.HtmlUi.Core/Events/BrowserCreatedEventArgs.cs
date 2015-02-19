using Samotorcan.HtmlUi.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Events
{
    /// <summary>
    /// Browser created event arguments.
    /// </summary>
    [CLSCompliant(false)]
    public class BrowserCreatedEventArgs : EventArgs
    {
        #region Properties
        #region Public

        #region CefBrowser
        /// <summary>
        /// Gets or sets the CEF browser.
        /// </summary>
        /// <value>
        /// The CEF browser.
        /// </value>
        public CefBrowser CefBrowser { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserCreatedEventArgs"/> class.
        /// </summary>
        /// <param name="cefBrowser">The cef browser.</param>
        /// <exception cref="System.ArgumentNullException">cefBrowser</exception>
        public BrowserCreatedEventArgs(CefBrowser cefBrowser)
        {
            Argument.Null(cefBrowser, "cefBrowser");

            CefBrowser = cefBrowser;
        }

        #endregion
    }
}
