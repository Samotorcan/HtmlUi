using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Browser.Handlers
{
    /// <summary>
    /// Request handler.
    /// </summary>
    [CLSCompliant(false)]
    public class RequestHandler : CefRequestHandler
    {
        #region Methods
        #region Protected

        #region OnBeforeBrowse
        /// <summary>
        /// Called on the UI thread before browser navigation. Return true to cancel
        /// the navigation or false to allow the navigation to proceed. The |request|
        /// object cannot be modified in this callback.
        /// CefLoadHandler::OnLoadingStateChange will be called twice in all cases.
        /// If the navigation is allowed CefLoadHandler::OnLoadStart and
        /// CefLoadHandler::OnLoadEnd will be called. If the navigation is canceled
        /// CefLoadHandler::OnLoadError will be called with an |errorCode| value of
        /// ERR_ABORTED.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="request"></param>
        /// <param name="isRedirect"></param>
        /// <returns></returns>
        protected override bool OnBeforeBrowse(CefBrowser browser, CefFrame frame, CefRequest request, bool isRedirect)
        {
            BaseMainApplication.Current.BrowserMessageRouter.OnBeforeBrowse(browser, frame);

            return false;
        }
        #endregion
        #region OnRenderProcessTerminated
        /// <summary>
        /// Called on the browser process UI thread when the render process
        /// terminates unexpectedly. |status| indicates how the process
        /// terminated.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="status"></param>
        protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        {
            BaseMainApplication.Current.BrowserMessageRouter.OnRenderProcessTerminated(browser);
        }
        #endregion
        #region GetResourceHandler
        /// <summary>
        /// Called on the IO thread before a resource is loaded. To allow the resource
        /// to load normally return NULL. To specify a handler for the resource return
        /// a CefResourceHandler object. The |request| object should not be modified in
        /// this callback.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override CefResourceHandler GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (BaseMainApplication.Current.IsContentUrl(request.Url))
                return new ContentResourceHandler();

            if (BaseMainApplication.Current.IsNativeRequestUrl(request.Url))
                return new NativeRequestResourceHandler();

            return null;
        }
        #endregion

        #endregion
        #endregion
    }
}
