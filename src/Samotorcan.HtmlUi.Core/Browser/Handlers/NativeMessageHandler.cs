using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Messages;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Samotorcan.HtmlUi.Core.Attributes;
using Newtonsoft.Json;

namespace Samotorcan.HtmlUi.Core.Browser.Handlers
{
    internal class NativeMessageHandler
    {
        private delegate object NativeFunctionDelegate(string json);
        private delegate void ProcessMessageDelegate(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage);

        private Dictionary<string, NativeFunctionDelegate> NativeFunctionDelegates { get; set; }
        private Dictionary<string, ProcessMessageDelegate> ProcessMessageDelegates { get; set; }

        public NativeMessageHandler()
        {
            NativeFunctionDelegates = NativeFunctionAttribute.GetHandlers<NativeMessageHandler, NativeFunctionDelegate>(this);
            ProcessMessageDelegates = ProcessMessageAttribute.GetHandlers<NativeMessageHandler, ProcessMessageDelegate>(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sourceProcess", Justification = "I want it to match to OnProcessMessageReceived method.")]
        public bool ProcessMessage(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            if (processMessage == null)
                throw new ArgumentNullException("processMessage");

            if (string.IsNullOrWhiteSpace(processMessage.Name))
                throw new ArgumentException("ProcessMessage.Name is null or white space.");

            Logger.Debug(string.Format("Process message: {0}", processMessage.Name));

            ProcessMessageDelegate handler;
            ProcessMessageDelegates.TryGetValue(processMessage.Name, out handler);

            if (handler != null)
            {
                handler(browser, sourceProcess, processMessage);

                return true;
            }

            return false;
        }

        [ProcessMessage("syncProperty")]
        private void ProcessMessageSyncProperty(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var syncProperty = MessageUtility.DeserializeMessage<SyncProperty>(processMessage);

            ChildApplication.Current.SetSyncProperty(syncProperty.Name, syncProperty.Value);
        }

        [ProcessMessage("callFunctionResult")]
        private void ProcessMessageCallFunction(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var message = MessageUtility.DeserializeMessage<CallFunctionResult>(processMessage);

            Application.Current.Window.SetCallFunctionResult(message.CallbackId, message.Result);
        }

        [ProcessMessage("native")]
        private void ProcessMessageNative(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var callNative = MessageUtility.DeserializeMessage<CallNative>(processMessage);

            Application.Current.InvokeOnMainAsync(() =>
            {
                object returnData = null;
                Exception exception = null;

                NativeFunctionDelegate handler;
                NativeFunctionDelegates.TryGetValue(callNative.Name, out handler);

                // function call
                if (handler != null)
                {
                    try
                    {
                        returnData = handler(callNative.Json);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                }
                else
                {
                    exception = new NativeNotFoundException(callNative.Name);
                }

                // callback
                if (callNative.CallbackId != null)
                {
                    var nativeResponse = new NativeResponse();

                    if (exception != null)
                    {
                        nativeResponse.Exception = ExceptionUtility.CreateJavascriptException(exception);
                        nativeResponse.Type = NativeResponseType.Exception;
                        nativeResponse.Value = null;
                    }
                    else
                    {
                        if (returnData == Value.Undefined)
                        {
                            nativeResponse.Exception = null;
                            nativeResponse.Type = NativeResponseType.Undefined;
                            nativeResponse.Value = null;
                        }
                        else
                        {
                            nativeResponse.Exception = null;
                            nativeResponse.Type = NativeResponseType.Value;
                            nativeResponse.Value = returnData;
                        }
                    }

                    var returnJson = JsonUtility.SerializeToJson(nativeResponse);

                    MessageUtility.SendMessage(CefProcessId.Renderer, browser, "native", new CallNativeResult { JsonResult = returnJson, CallbackId = callNative.CallbackId });
                }
            }).ContinueWith(t =>
            {
                Logger.Error("Native call exception.", t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        [NativeFunction("syncControllerChanges")]
        private object NativeFunctionSyncControllerChanges(string json)
        {
            var controllerChanges = JsonConvert.DeserializeObject<List<ControllerChange>>(json);

            Application.Current.Window.SyncControllerChangesToNative(controllerChanges);

            return Value.Undefined;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "json", Justification = "It has to match to the delegate.")]
        [NativeFunction("getControllerNames")]
        private object NativeFunctionGetControllerNames(string json)
        {
            return Application.Current.GetControllerNames();
        }

        [NativeFunction("createController")]
        private object NativeFunctionCreateController(string json)
        {
            var createController = JsonConvert.DeserializeObject<CreateController>(json);
            var controller = Application.Current.Window.CreateController(createController.Name);

            return controller.GetControllerDescription();
        }

        [NativeFunction("createObservableController")]
        private object NativeFunctionCreateObservableController(string json)
        {
            var createController = JsonConvert.DeserializeObject<CreateController>(json);
            var observableController = Application.Current.Window.CreateObservableController(createController.Name);

            return observableController.GetObservableControllerDescription();
        }

        [NativeFunction("destroyController")]
        private object NativeFunctionDestroyController(string json)
        {
            var controllerId = JsonConvert.DeserializeObject<int>(json);

            Application.Current.Window.DestroyController(controllerId);

            return Value.Undefined;
        }

        [NativeFunction("callMethod")]
        private object NativeFunctionCallMethod(string json)
        {
            var methodData = JsonConvert.DeserializeObject<CallMethod>(json);

            return Application.Current.Window.CallMethod(methodData.Id, methodData.Name, methodData.Arguments, methodData.InternalMethod);
        }
    }
}
