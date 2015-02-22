using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using Samotorcan.HtmlUi.Core.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Argument.Null(response, "response");

            redirectUrl = null;
            response.MimeType = "text/html";    // TODO: set proper mime type

            if (Exception != null)
            {
                Data = Encoding.UTF8.GetBytes(string.Format(ResourceUtility.GetResourceAsString("Views/ContentRequestException.html"),
                    Url,
                    Exception.ToString().Replace(Environment.NewLine, "<br>")));

                responseLength = Data.Length;
                response.Status = 500;
                response.StatusText = "Internal Server Error";
            }
            else
            {
                responseLength = Data.Length;
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

            application.InvokeOnMainAsync(() =>
            {
                try
                {
                    var path = application.GetContentPath(request.Url);
                    var fileExtension = Path.GetExtension(path).TrimStart('.');
                    var content = application.ContentProvider.GetContent(path);

                    // html content
                    if (!string.IsNullOrWhiteSpace(fileExtension) && application.HtmlFileExtensions.Contains(fileExtension))
                    {
                        Data = ProcessHtmlFile(content);
                    }

                    // unknown content
                    else
                    {
                        Data = content;
                    }
                }
                catch (Exception e)
                {
                    Data = null;
                    Exception = e;

                    GeneralLog.Error("Content request exception.", e);
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
            Argument.Null(response, "response");

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
        #region Private

        #region ProcessHtmlFile
        /// <summary>
        /// Processes the HTML file.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private byte[] ProcessHtmlFile(byte[] data)
        {
            Argument.Null(data, "data");

            return data;
        }
        #endregion

        #endregion
        #endregion
    }
}
