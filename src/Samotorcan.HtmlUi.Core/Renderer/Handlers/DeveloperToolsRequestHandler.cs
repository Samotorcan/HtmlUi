using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Samotorcan.HtmlUi.Core.Logs;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Renderer.Handlers
{
    /// <summary>
    /// Developer tools request handler.
    /// </summary>
    internal class DeveloperToolsRequestHandler : CefRequestHandler
    {
        #region Methods
        #region Protected

        #region GetResourceHandler
        /// <summary>
        /// Gets the resource handler.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected override CefResourceHandler GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            Logger.Debug(string.Format("Developer tools request: {0}", request.Url));

            return null;
        }
        #endregion

        #endregion
        #endregion
    }
}
