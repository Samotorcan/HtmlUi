using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using Samotorcan.HtmlUi.Core.Validation;
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
    /// Local CEF resource handler.
    /// </summary>
    internal class LocalCefResourceHandler : CefResourceHandler
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
        #region Content
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        private byte[] Content { get; set; }
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
            Argument.Null(response, "response");

            redirectUrl = null;
            response.MimeType = "text/html";

            if (Exception != null)
            {
                Content = Encoding.UTF8.GetBytes(string.Format(ResourceUtility.GetResourceAsString("ViewRequestException.html"),
                    Url,
                    Exception.ToString().Replace(Environment.NewLine, "<br>")));

                responseLength = Content.Length;
                response.Status = 500;
                response.StatusText = "Internal Server Error";
            }
            else
            {
                responseLength = Content.Length;
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
            Argument.Null(request, "request");

            Url = request.Url;
            var application = BaseApplication.Current;

            // view url
            if (application.IsViewFileUrl(Url))
            {
                application.InvokeOnMainAsync(() =>
                {
                    try
                    {
                        var viewPath = application.GetViewPath(request.Url);
                        Content = Encoding.UTF8.GetBytes(application.ViewProvider.GetView(viewPath));
                    }
                    catch (Exception e)
                    {
                        Content = null;
                        Exception = e;

                        GeneralLog.Error("View request exception.", e);
                    }

                    callback.Continue();
                });
            }

            // unknown url
            else
            {
                Exception = new UnknownUrlException();
                callback.Continue();
            }

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
            Argument.Null(response, "response");

            bytesRead = 0;

            if (AllBytesRead >= Content.Length)
                return false;

            bytesRead = Math.Min(bytesToRead, Content.Length - AllBytesRead);
            response.Write(Content, AllBytesRead, bytesRead);

            AllBytesRead += bytesRead;

            return true;
        }
        #endregion

        #endregion
        #endregion
    }
}
