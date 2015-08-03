using System;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Events
{
    /// <summary>
    /// On load start event arguments.
    /// </summary>
    internal class OnLoadStartEventArgs : EventArgs
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

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OnLoadStartEventArgs"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="frame">The frame.</param>
        public OnLoadStartEventArgs(CefBrowser browser, CefFrame frame)
        {
            Browser = browser;
            Frame = frame;
        }

        #endregion
    }
}
