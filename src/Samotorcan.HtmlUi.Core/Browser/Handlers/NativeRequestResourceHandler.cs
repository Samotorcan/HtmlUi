using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Messages;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Xilium.CefGlue;
using Samotorcan.HtmlUi.Core.Attributes;

namespace Samotorcan.HtmlUi.Core.Browser.Handlers
{
    internal class NativeRequestResourceHandler : CefResourceHandler
    {
        private string Url { get; set; }
        public string Path { get; set; }
        private Exception Exception { get; set; }
        private byte[] Data { get; set; }
        private object ResponseValue { get; set; }
        private int AllBytesRead { get; set; }

        private delegate object NativeFunctionDelegate(CefRequest request);
        private Dictionary<string, NativeFunctionDelegate> NativeFunctions { get; set; }

        private Application Application
        {
            get
            {
                return Application.Current;
            }
        }

        public NativeRequestResourceHandler()
            : base()
        {
            NativeFunctions = NativeFunctionAttribute.GetHandlers<NativeRequestResourceHandler, NativeFunctionDelegate>(this);
        }

        protected override bool CanGetCookie(CefCookie cookie)
        {
            return false;
        }

        protected override bool CanSetCookie(CefCookie cookie)
        {
            return false;
        }

        protected override void Cancel() { }

        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            redirectUrl = null;

            response.Status = 200;
            response.StatusText = "OK";
            response.MimeType = "application/json";
            response.SetHeaderMap(new NameValueCollection { { "Access-Control-Allow-Origin", string.Format("http://{0}", Application.RequestHostname) } });

            var nativeResponse = new NativeResponse();

            if (Exception != null)
            {
                nativeResponse.Type = NativeResponseType.Exception;
                nativeResponse.Value = null;
                nativeResponse.Exception = ExceptionUtility.CreateJavascriptException(Exception);
            }
            else
            {
                if (ResponseValue == Value.Undefined)
                {
                    nativeResponse.Type = NativeResponseType.Undefined;
                    nativeResponse.Value = null;
                }
                else
                {
                    nativeResponse.Type = NativeResponseType.Value;
                    nativeResponse.Value = ResponseValue;
                }

                nativeResponse.Exception = null;
            }

            Data = JsonUtility.SerializeToByteJson(nativeResponse);
            responseLength = Data.Length;
        }

        protected override bool ProcessRequest(CefRequest request, CefCallback callback)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (callback == null)
                throw new ArgumentNullException("callback");

            Url = request.Url;
            Path = Application.GetNativeRequestPath(Url);

            Logger.Debug(string.Format("Native request: {0}", Url));

            try
            {
                var nativeFunction = FindNativeFunction(Path);

                if (nativeFunction != null)
                    ResponseValue = nativeFunction(request);
                else
                    Exception = new NativeNotFoundException(Path);
            }
            catch (Exception e)
            {
                ResponseValue = null;
                Exception = e;
            }

            callback.Continue();

            return true;
        }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "request", Justification = "It has to match to the delegate.")]
        [NativeFunction("getControllerNames")]
        private object NativeFunctionGetControllerNames(CefRequest request)
        {
            List<string> controllerNames = null;

            Application.InvokeOnMain(() =>
            {
                controllerNames = Application.GetControllerNames();
            });

            return controllerNames;
        }

        [NativeFunction("createController")]
        private object NativeFunctionCreateController(CefRequest request)
        {
            var controllerData = GetPostData<CreateController>(request);
            ControllerDescription controllerDescription = null;

            Application.InvokeOnMain(() =>
            {
                var controller = Application.Window.CreateController(controllerData.Name);

                controllerDescription = controller.GetControllerDescription();

                // camel case method names
                foreach (var method in controllerDescription.Methods)
                    method.Name = StringUtility.CamelCase(method.Name);
            });

            return controllerDescription;
        }

        [NativeFunction("createObservableController")]
        private object NativeFunctionCreateObservableController(CefRequest request)
        {
            var controllerData = GetPostData<CreateController>(request);
            ObservableControllerDescription observableControllerDescription = null;

            Application.InvokeOnMain(() =>
            {
                var observableController = Application.Window.CreateObservableController(controllerData.Name);

                observableControllerDescription = observableController.GetObservableControllerDescription();

                // camel case property and method names
                foreach (var property in observableControllerDescription.Properties)
                    property.Name = StringUtility.CamelCase(property.Name);

                foreach (var method in observableControllerDescription.Methods)
                    method.Name = StringUtility.CamelCase(method.Name);
            });

            return observableControllerDescription;
        }

        [NativeFunction("destroyController")]
        private object NativeFunctionDestroyController(CefRequest request)
        {
            var controllerId = GetPostData<int>(request);

            Application.InvokeOnMain(() =>
            {
                Application.Window.DestroyController(controllerId);
            });

            return Value.Undefined;
        }

        [NativeFunction("syncControllerChanges")]
        private object NativeFunctionSyncControllerChanges(CefRequest request)
        {
            var controllerChanges = GetPostData<List<ControllerChange>>(request);

            Application.InvokeOnMain(() =>
            {
                Application.Window.SyncControllerChangesToNative(controllerChanges);
            });

            return Value.Undefined;
        }

        [NativeFunction("callMethod")]
        private object NativeFunctionCallMethod(CefRequest request)
        {
            var methodData = GetPostData<CallMethod>(request);
            object response = null;

            Application.InvokeOnMain(() =>
            {
                response = Application.Window.CallMethod(methodData.Id, methodData.Name, methodData.Arguments, methodData.InternalMethod);
            });

            return response;
        }

        [NativeFunction("log")]
        private object NativeFunctionLog(CefRequest request)
        {
            var jsonToken = GetPostJsonToken(request);

            var type = LogType.Parse((string)jsonToken["type"]);
            var messageType = LogMessageType.Parse((string)jsonToken["messageType"]);
            var message = jsonToken["message"].ToString();

            if (type == LogType.Logger)
                Logger.Log(messageType, message);
            else
                throw new ArgumentException("Invalid log type.");

            return Value.Undefined;
        }

        private TData GetPostData<TData>(CefRequest request)
        {
            var json = GetPostJson(request);

            return JsonConvert.DeserializeObject<TData>(json);
        }

        private TData GetAnonymousPostData<TData>(CefRequest request, TData anonymousObject)
        {
            var json = GetPostJson(request);

            return JsonConvert.DeserializeAnonymousType(json, anonymousObject);
        }

        private string GetPostJson(CefRequest request)
        {
            return Encoding.UTF8.GetString(request.PostData.GetElements()[0].GetBytes());
        }

        private JToken GetPostJsonToken(CefRequest request)
        {
            var json = GetPostJson(request);

            return JToken.Parse(json);
        }

        private NativeFunctionDelegate FindNativeFunction(string name)
        {
            NativeFunctionDelegate nativeFunction = null;

            if (NativeFunctions.TryGetValue(name, out nativeFunction))
                return nativeFunction;

            if (NativeFunctions.TryGetValue(StringUtility.PascalCase(name), out nativeFunction))
                return nativeFunction;

            if (NativeFunctions.TryGetValue(StringUtility.CamelCase(name), out nativeFunction))
                return nativeFunction;

            return null;
        }
    }
}
