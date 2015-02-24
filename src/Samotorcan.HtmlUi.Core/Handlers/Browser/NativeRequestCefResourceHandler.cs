using Newtonsoft.Json;
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

namespace Samotorcan.HtmlUi.Core.Handlers.Browser
{
    /// <summary>
    /// Native request CEF resource handler.
    /// </summary>
    internal class NativeRequestCefResourceHandler : CefResourceHandler
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
                response.SetHeaderMap(new NameValueCollection { { "Access-Control-Allow-Origin", "*" } });
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
            Path = BaseApplication.Current.GetNativeRequestPath(Url);

            try
            {
                if (Path == "create-controllers")
                    Data = CreateControllers();
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

        #region CreateControllers
        /// <summary>
        /// Creates the controllers and returns a JSON description of the controllers.
        /// </summary>
        /// <returns></returns>
        private byte[] CreateControllers()
        {
            var application = BaseApplication.Current;
            var window = application.Window;
            var controllerProvider = application.ControllerProvider;
            var controllerTypes = controllerProvider.GetControllerTypes();

            var createdControllers = new List<Controller>();

            // create controllers
            application.InvokeOnMain(() => {
                try
                {
                    foreach (var controllerType in controllerTypes)
                    {
                        createdControllers.Add(controllerProvider.CreateController(controllerType.Name));
                    }
                }
                catch (Exception)
                {
                    foreach (var createdController in createdControllers)
                        createdController.Dispose();

                    throw;
                }

                // dispose current controllers
                foreach (var controller in window.Controllers)
                    controller.Dispose();

                // save created controllers
                window.Controllers = createdControllers;
            });

            var controllerDescriptions = window.Controllers
                .Select(c => c.GetDescription(PropertyNameType.CamelCase))
                .ToArray();

            return JsonUtility.SerializeToJson(controllerDescriptions);
        }
        #endregion

        

        #endregion
        #endregion
    }
}
