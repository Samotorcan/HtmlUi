using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.IO;
using System.Text;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Browser.Handlers
{
    /// <summary>
    /// Content resource handler.
    /// </summary>
    internal class ContentResourceHandler : CefResourceHandler
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
        #region Data
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        private byte[] Data { get; set; }
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
        /// Gets the response headers.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="responseLength"></param>
        /// <param name="redirectUrl"></param>
        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            redirectUrl = null;

            if (Exception != null)
            {
                Data = Encoding.UTF8.GetBytes(string.Format(ResourceUtility.GetResourceAsString("Views/ContentRequestException.html"),
                    Url,
                    Exception.ToString().Replace(Environment.NewLine, "<br>")));

                responseLength = Data.Length;
                response.Status = 500;
                response.StatusText = "Internal Server Error";
                response.MimeType = "text/html";
            }
            else if (Data != null)
            {
                responseLength = Data.Length;
                response.Status = 200;
                response.StatusText = "OK";

                // mime type
                response.MimeType = "application/octet-stream";
                var extension = Path.GetExtension(Url);

                if (!string.IsNullOrWhiteSpace(extension) && Application.Current.MimeTypes.ContainsKey(extension))
                    response.MimeType = Application.Current.MimeTypes[extension];
            }
            else
            {
                Data = Encoding.UTF8.GetBytes(string.Format(ResourceUtility.GetResourceAsString("Views/ContentNotFound.html"), Url));

                responseLength = Data.Length;
                response.Status = 404;
                response.StatusText = "Not Found";
                response.MimeType = "text/html";
            }
        }
        #endregion
        #region ProcessRequest
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected override bool ProcessRequest(CefRequest request, CefCallback callback)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            Url = request.Url;
            var application = Application.Current;

            application.InvokeOnMainAsync(() =>
            {
                try
                {
                    var path = application.GetContentPath(request.Url);

                    if (application.ContentProvider.ContentExists(path))
                        Data = application.ContentProvider.GetContent(path);
                }
                catch (Exception e)
                {
                    Data = null;
                    Exception = e;

                    Logger.Error("Content request exception.", e);
                }

                callback.Continue();
            });

            return true;
        }
        #endregion
        #region ReadResponse
        /// <summary>
        /// Reads the response.
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

            if (AllBytesRead >= Data.Length)
                return false;

            bytesRead = Math.Min(bytesToRead, Data.Length - AllBytesRead);
            response.Write(Data, AllBytesRead, bytesRead);

            AllBytesRead += bytesRead;

            return true;
        }
        #endregion

        #endregion
        #endregion
    }
}
