using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Scheme
{
    /// <summary>
    /// Default app schema handler factory.
    /// </summary>
    [CLSCompliant(false)]
    public class DefaultAppSchemeHandlerFactory : CefSchemeHandlerFactory
    {
        /// <summary>
        /// Return a new resource handler instance to handle the request or an empty
        /// reference to allow default handling of the request. |browser| and |frame|
        /// will be the browser window and frame respectively that originated the
        /// request or NULL if the request did not originate from a browser window
        /// (for example, if the request came from CefURLRequest). The |request| object
        /// passed to this method will not contain cookie data.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="schemeName"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
        {
            return new DefaultRequestResourceHandler();
        }
    }
}
