using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Messages;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using Xilium.CefGlue;
using Samotorcan.HtmlUi.Core.Attributes;

namespace Samotorcan.HtmlUi.Core.Renderer.Handlers
{
    internal class NativeMessageHandler : CefV8Handler
    {
        public CefBrowser CefBrowser { get; set; }

        private delegate void JavascriptFunctionDelegate(CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue);
        private delegate void ProcessMessageDelegate(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage);

        private Dictionary<string, JavascriptFunctionDelegate> JavascriptFunctionDelegates { get; set; }
        private Dictionary<string, ProcessMessageDelegate> ProcessMessageDelegates { get; set; }

        private Dictionary<Guid, JavascriptFunction> Callbacks { get; set; }
        private Dictionary<string, JavascriptFunction> Functions { get; set; }

        private ChildApplication Application
        {
            get
            {
                return ChildApplication.Current;
            }
        }

        public NativeMessageHandler()
        {
            Callbacks = new Dictionary<Guid, JavascriptFunction>();
            Functions = new Dictionary<string, JavascriptFunction>();

            JavascriptFunctionDelegates = JavascriptFunctionAttribute.GetHandlers<NativeMessageHandler, JavascriptFunctionDelegate>(this);
            ProcessMessageDelegates = ProcessMessageAttribute.GetHandlers<NativeMessageHandler, ProcessMessageDelegate>(this);
        }

        public void Reset()
        {
            Callbacks.Clear();
            Functions.Clear();
        }

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

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            Logger.Debug(string.Format("Javascript function call: {0}", name));

            returnValue = null;
            exception = null;

            JavascriptFunctionDelegate handler;
            JavascriptFunctionDelegates.TryGetValue(name, out handler);

            if (handler != null)
            {
                try
                {
                    handler(obj, arguments, out returnValue);
                }
                catch (Exception e)
                {
                    exception = e.Message;
                }

                return true;
            }

            return false;
        }

        private Guid? AddCallback(CefV8Value callbackFunction, CefV8Context context)
        {
            if (callbackFunction != null)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                if (!callbackFunction.IsFunction)
                    throw new ArgumentException("Not a function.", "callbackFunction");

                var callback = new JavascriptFunction(callbackFunction, context);
                Callbacks.Add(callback.Id, callback);

                return callback.Id;
            }

            return null;
        }

        private JavascriptFunction GetCallback(Guid id)
        {
            JavascriptFunction callback;
            Callbacks.TryGetValue(id, out callback);

            if (callback != null)
            {
                Callbacks.Remove(id);
                return callback;
            }

            return null;
        }

        [JavascriptFunction("native")]
        private void JavascriptFunctionNative(CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            if (arguments.Length < 2 || !arguments[0].IsString || !arguments[1].IsString || arguments.Length > 2 && !arguments[2].IsFunction)
                throw new ArgumentException("Invalid arguments.", "arguments");

            returnValue = null;

            var functionName = arguments[0].GetStringValue();
            var jsonData = arguments[1].GetStringValue();
            var callbackFunction = arguments.Length > 2 && arguments[2].IsFunction ? arguments[2] : null;

            var callbackId = AddCallback(callbackFunction, CefV8Context.GetCurrentContext());

            MessageUtility.SendMessage(CefProcessId.Browser, CefBrowser, "native", new CallNative { Name = functionName, Json = jsonData, CallbackId = callbackId });
        }

        [ProcessMessage("native")]
        private void ProcessMessageNative(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var callNativeResult = MessageUtility.DeserializeMessage<CallNativeResult>(processMessage);

            if (callNativeResult.CallbackId != null)
            {
                var callback = GetCallback(callNativeResult.CallbackId.Value);

                if (callback == null)
                    throw new InvalidOperationException(string.Format("Callback '{0}' not found.", callNativeResult.CallbackId.Value));

                callback.Execute(callNativeResult.JsonResult);
            }
        }

        [JavascriptFunction("registerFunction")]
        private void JavascriptFunctionRegisterFunction(CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            if (arguments.Length < 2 || !arguments[0].IsString || !arguments[1].IsFunction)
                throw new ArgumentException("Invalid arguments.", "arguments");

            returnValue = null;

            var functionName = arguments[0].GetStringValue();
            var function = arguments[1];

            if (Functions.ContainsKey(functionName))
                throw new InvalidOperationException(string.Format("Function '{0}' is already registered.", functionName));

            Functions.Add(functionName, new JavascriptFunction(function, CefV8Context.GetCurrentContext()));
        }

        [ProcessMessage("callFunction")]
        private void ProcessMessageCallFunction(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var callFunction = MessageUtility.DeserializeMessage<CallFunction>(processMessage);

            if (!Functions.ContainsKey(callFunction.Name))
                throw new InvalidOperationException(string.Format("Function '{0}' not found.", callFunction.Name));

            JToken returnValue = null;

            if (callFunction.Data != Value.Undefined)
                returnValue = Functions[callFunction.Name].Execute(callFunction.Data);
            else
                returnValue = Functions[callFunction.Name].Execute();

            MessageUtility.SendMessage(CefProcessId.Browser, CefBrowser, "callFunctionResult", new CallFunctionResult { Result = returnValue, CallbackId = callFunction.CallbackId });
        }

        [JavascriptFunction("loadInternalScript")]
        private void JavascriptFunctionLoadInternalScript(CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            if (arguments.Length < 1 || !arguments[0].IsString)
                throw new ArgumentException("Invalid arguments.", "arguments");

            returnValue = null;

            var scriptName = "Scripts/" + arguments[0].GetStringValue();

            if (!ResourceUtility.ResourceExists(scriptName))
                throw new InvalidOperationException(string.Format("Script '{0}' not found.", scriptName));

            CefV8Value evalReturnValue = null;
            CefV8Exception evalException = null;

            string script = ResourceUtility.GetResourceAsString(scriptName);
            var context = CefV8Context.GetCurrentContext();

            if (!context.TryEval(script, out evalReturnValue, out evalException))
                throw new InvalidOperationException(string.Format("Javascript exception: {0}.", JsonConvert.SerializeObject(evalException)));
        }

        [ProcessMessage("syncProperty")]
        private void ProcessMessageSyncProperty(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var syncProperty = MessageUtility.DeserializeMessage<SyncProperty>(processMessage);

            Application.SetSyncProperty(syncProperty.Name, syncProperty.Value);
        }
    }
}
