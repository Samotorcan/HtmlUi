using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Handlers.Browser
{
    /// <summary>
    /// View CEF resource handler.
    /// </summary>
    internal class ViewCefResourceHandler : CefResourceHandler
    {
        #region Properties
        #region Private

        #region Url
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        private string Url { get; set; }
        #endregion
        #region Exception
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        private Exception Exception { get; set; }
        #endregion
        #region ViewPath
        /// <summary>
        /// Gets or sets the view path.
        /// </summary>
        /// <value>
        /// The view path.
        /// </value>
        private string ViewPath { get; set; }
        #endregion
        #region ViewContent
        /// <summary>
        /// Gets or sets the content of the view.
        /// </summary>
        /// <value>
        /// The content of the view.
        /// </value>
        private byte[] ViewContent { get; set; }
        #endregion
        #region AllBytesRead
        /// <summary>
        /// Gets or sets all bytes read.
        /// </summary>
        /// <value>
        /// All bytes read.
        /// </value>
        private int AllBytesRead { get; set; }
        #endregion

        #endregion
        #endregion
        #region Methods
        #region Protected

        #region CanGetCookie
        /// <summary>
        /// Cookies are disabled, returns false.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        protected override bool CanGetCookie(CefCookie cookie)
        {
            return false;
        }
        #endregion
        #region CanSetCookie
        /// <summary>
        /// Cookies are disabled, returns false.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        protected override bool CanSetCookie(CefCookie cookie)
        {
            return false;
        }
        #endregion
        #region Cancel
        /// <summary>
        /// Request processing has been canceled.
        /// </summary>
        protected override void Cancel() { }
        #endregion
        #region GetResponseHeaders
        /// <summary>
        /// Gets the response headers for the view.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="responseLength"></param>
        /// <param name="redirectUrl"></param>
        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            redirectUrl = null;
            response.MimeType = "text/html";

            if (Exception != null)
            {
                ViewContent = Encoding.UTF8.GetBytes(string.Format(ResourceUtility.GetResourceAsString("ViewRequestException.html"),
                    Url,
                    Exception.ToString().Replace(Environment.NewLine, "<br>")));

                responseLength = ViewContent.Length;
                response.Status = 404;
                response.StatusText = "Not Found";
            }
            else
            {
                responseLength = ViewContent.Length;
                response.Status = 200;
                response.StatusText = "OK";
            }
        }
        #endregion
        #region ProcessRequest
        /// <summary>
        /// Processes the request for the view.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected override bool ProcessRequest(CefRequest request, CefCallback callback)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            Url = request.Url;

            BaseApplication.Current.InvokeOnMainAsync(() => {
                var application = BaseApplication.Current;

                try
                {
                    ViewPath = application.GetViewPath(request.Url);
                    ViewContent = Encoding.UTF8.GetBytes(application.ViewProvider.GetView(ViewPath));
                        
                }
                catch (Exception e)
                {
                    ViewPath = null;
                    ViewContent = null;
                    Exception = e;

                    GeneralLog.Error("View request exception.", e);
                }

                callback.Continue();
            });

            return true;
        }
        #endregion
        #region ReadResponse
        /// <summary>
        /// Returns the view content.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="bytesToRead"></param>
        /// <param name="bytesRead"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected override bool ReadResponse(Stream response, int bytesToRead, out int bytesRead, CefCallback callback)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            bytesRead = 0;

            if (AllBytesRead >= ViewContent.Length)
                return false;

            bytesRead = Math.Min(bytesToRead, ViewContent.Length - AllBytesRead);
            response.Write(ViewContent, AllBytesRead, bytesRead);

            AllBytesRead += bytesRead;

            return true;
        }
        #endregion

        #endregion
        #endregion
    }
}
