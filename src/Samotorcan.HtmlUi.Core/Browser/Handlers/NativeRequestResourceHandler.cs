using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Browser.Handlers
{
    /// <summary>
    /// Native request resource handler.
    /// </summary>
    internal class NativeRequestResourceHandler : CefResourceHandler
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
        #region Path
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }
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

            response.SetHeaderMap(new NameValueCollection { { "Access-Control-Allow-Origin", "*" } });

            if (Exception != null)
            {
                Data = Encoding.UTF8.GetBytes(string.Format(ResourceUtility.GetResourceAsString("Views/NativeRequestException.html"),
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
                response.MimeType = "application/json";
            }
            else
            {
                Data = Encoding.UTF8.GetBytes(string.Format(ResourceUtility.GetResourceAsString("Views/NativeRequestNotFound.html"), Url));

                responseLength = Data.Length;
                response.Status = 404;
                response.StatusText = "Not Found";
                response.MimeType = "text/html";
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
            Path = BaseMainApplication.Current.GetNativeRequestPath(Url);

            try
            {
                switch (Path)
                {
                    case "controller-names":
                        Data = ControllerNames(request);
                        break;
                    case "create-controller":
                        Data = CreateController(request);
                        break;
                    case "digest":
                        Data = Digest(request);
                        break;
                    case "log":
                        Data = Log(request);
                        break;
                }
            }
            catch (Exception e)
            {
                Data = null;
                Exception = e;

                GeneralLog.Error("Native request exception.", e);
            }

            callback.Continue();

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
        #region Private

        #region ControllerNames
        /// <summary>
        /// Controller names.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private byte[] ControllerNames(CefRequest request)
        {
            IEnumerable<Type> controllerTypes = null;

            BaseMainApplication.Current.InvokeOnMain(() =>
            {
                controllerTypes = BaseMainApplication.Current.ControllerProvider.GetControllerTypes();
            });

            return JsonUtility.SerializeToJson(controllerTypes.Select(c => c.Name).ToList());
        }
        #endregion
        #region CreateController
        /// <summary>
        /// Controller names.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private byte[] CreateController(CefRequest request)
        {
            var controllerData = GetAnonymousPostData(request, new { Name = string.Empty, id = 0 });
            ControllerDescription controllerDescription = null;

            BaseMainApplication.Current.InvokeOnMain(() =>
            {
                var controller = BaseMainApplication.Current.Window.CreateController(controllerData.Name, controllerData.id);

                controllerDescription = controller.GetDescription();
            });

            return JsonUtility.SerializeToJson(controllerDescription);
        }
        #endregion
        #region Digest
        /// <summary>
        /// Calls the digest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private byte[] Digest(CefRequest request)
        {
            var controllerChanges = GetPostData<List<ControllerChange>>(request);

            var application = BaseMainApplication.Current;
            application.InvokeOnMain(() =>
            {
                application.Window.Digest(controllerChanges);
            });

            return new byte[0];
        }
        #endregion
        #region Log
        /// <summary>
        /// Calls the log.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private byte[] Log(CefRequest request)
        {
            var jsonToken = GetPostJsonToken(request);

            var type = LogType.Parse((string)jsonToken["type"]);
            var messageType = LogMessageType.Parse((string)jsonToken["messageType"]);
            var message = jsonToken["message"].ToString();

            if (type == LogType.GeneralLog)
                GeneralLog.Log(messageType, message);

            return new byte[0];
        }
        #endregion

        #region GetPostData
        /// <summary>
        /// Gets the post data.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private TData GetPostData<TData>(CefRequest request)
        {
            var json = GetPostJson(request);

            return JsonConvert.DeserializeObject<TData>(json);
        }
        #endregion
        #region GetAnonymousPostData
        /// <summary>
        /// Gets the anonymous post data.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="anonymousObject">The anonymous object.</param>
        /// <returns></returns>
        private TData GetAnonymousPostData<TData>(CefRequest request, TData anonymousObject)
        {
            var json = GetPostJson(request);

            return JsonConvert.DeserializeAnonymousType(json, anonymousObject);
        }
        #endregion
        #region GetPostJson
        /// <summary>
        /// Gets the post json.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private string GetPostJson(CefRequest request)
        {
            return Encoding.UTF8.GetString(request.PostData.GetElements()[0].GetBytes());
        }
        #endregion
        #region GetPostJsonToken
        /// <summary>
        /// Gets the post json token.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private JToken GetPostJsonToken(CefRequest request)
        {
            var json = GetPostJson(request);

            return JToken.Parse(json);
        }
        #endregion

        #endregion
        #endregion
    }
}
