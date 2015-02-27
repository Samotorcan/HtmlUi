using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Scheme
{
    /// <summary>
    /// Default request resource handler.
    /// </summary>
    [CLSCompliant(false)]
    public class DefaultRequestResourceHandler : CefResourceHandler
    {
        private static int _requestNo;

        private byte[] responseData;
        private int pos;


        /// <summary>
        /// Begin processing the request. To handle the request return true and call
        /// CefCallback::Continue() once the response header information is available
        /// (CefCallback::Continue() can also be called from inside this method if
        /// header information is available immediately). To cancel the request return
        /// false.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected override bool ProcessRequest(CefRequest request, CefCallback callback)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (callback == null)
                throw new ArgumentNullException("callback");

            var requestNo = Interlocked.Increment(ref _requestNo);

            var response = new StringBuilder();

            response.AppendFormat("<pre>\n");
            response.AppendFormat("Requests processed by DemoAppResourceHandler: {0}\n", requestNo);

            response.AppendFormat("Method: {0}\n", request.Method);
            response.AppendFormat("URL: {0}\n", request.Url);

            response.AppendLine();
            response.AppendLine("Headers:");
            var headers = request.GetHeaderMap();
            foreach (string key in headers)
            {
                foreach (var value in headers.GetValues(key))
                {
                    response.AppendFormat("{0}: {1}\n", key, value);
                }
            }
            response.AppendLine();

            response.AppendFormat("</pre>\n");

            responseData = Encoding.UTF8.GetBytes(response.ToString());

            callback.Continue();
            return true;
        }

        /// <summary>
        /// Retrieve response header information. If the response length is not known
        /// set |response_length| to -1 and ReadResponse() will be called until it
        /// returns false. If the response length is known set |response_length|
        /// to a positive value and ReadResponse() will be called until it returns
        /// false or the specified number of bytes have been read. Use the |response|
        /// object to set the mime type, http status code and other optional header
        /// values. To redirect the request to a new URL set |redirectUrl| to the new
        /// URL.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="responseLength"></param>
        /// <param name="redirectUrl"></param>
        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            response.MimeType = "text/html";
            response.Status = 200;
            response.StatusText = "OK, hello from handler!";

            var headers = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            headers.Add("Cache-Control", "private");
            response.SetHeaderMap(headers);

            responseLength = responseData.LongLength;
            redirectUrl = null;
        }

        /// <summary>
        /// Read response data. If data is available immediately copy up to
        /// |bytes_to_read| bytes into |data_out|, set |bytes_read| to the number of
        /// bytes copied, and return true. To read the data at a later time set
        /// |bytes_read| to 0, return true and call CefCallback::Continue() when the
        /// data is available. To indicate response completion return false.
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

            if (bytesToRead == 0 || pos >= responseData.Length)
            {
                bytesRead = 0;
                return false;
            }
            else
            {
                response.Write(responseData, pos, bytesToRead);
                pos += bytesToRead;
                bytesRead = bytesToRead;
                return true;
            }
        }

        /// <summary>
        /// Return true if the specified cookie can be sent with the request or false
        /// otherwise. If false is returned for any cookie then no cookies will be sent
        /// with the request.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        protected override bool CanGetCookie(CefCookie cookie)
        {
            return false;
        }

        /// <summary>
        /// Return true if the specified cookie returned with the response can be set
        /// or false otherwise.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        protected override bool CanSetCookie(CefCookie cookie)
        {
            return false;
        }

        /// <summary>
        /// Request processing has been canceled.
        /// </summary>
        protected override void Cancel()
        {
        }
    }
}
