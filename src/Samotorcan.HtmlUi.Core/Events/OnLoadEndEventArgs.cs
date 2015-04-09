using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Events
{
    /// <summary>
    /// On load end event arguments.
    /// </summary>
    internal class OnLoadEndEventArgs : EventArgs
    {
        #region Properties
        #region Public

        #region Browser
        /// <summary>
        /// Gets the browser.
        /// </summary>
        /// <value>
        /// The browser.
        /// </value>
        public CefBrowser Browser { get; private set; }
        #endregion
        #region Frame
        /// <summary>
        /// Gets the frame.
        /// </summary>
        /// <value>
        /// The frame.
        /// </value>
        public CefFrame Frame { get; private set; }
        #endregion
        #region HttpStatusCode
        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <value>
        /// The HTTP status code.
        /// </value>
        public int HttpStatusCode { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OnLoadEndEventArgs"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public OnLoadEndEventArgs(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            Browser = browser;
            Frame = frame;
            HttpStatusCode = httpStatusCode;
        }

        #endregion
    }
}
