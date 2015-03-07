using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Browser.Handlers;
using Samotorcan.HtmlUi.Core.Events;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Samotorcan.HtmlUi.Core.Browser
{
    /// <summary>
    /// Client.
    /// </summary>
    [CLSCompliant(false)]
    public class Client : CefClient
    {
        #region Events

        #region BrowserCreated
        /// <summary>
        /// Occurs when the browser is created.
        /// </summary>
        public event EventHandler<BrowserCreatedEventArgs> BrowserCreated;
        #endregion

        #endregion
        #region Properties
        #region Private

        #region LifeSpanHandler
        /// <summary>
        /// Gets or sets the life span handler.
        /// </summary>
        /// <value>
        /// The life span handler.
        /// </value>
        private LifeSpanHandler LifeSpanHandler { get; set; }
        #endregion
        #region DisplayHandler
        /// <summary>
        /// Gets or sets the display handler.
        /// </summary>
        /// <value>
        /// The display handler.
        /// </value>
        private DisplayHandler DisplayHandler { get; set; }
        #endregion
        #region LoadHandler
        /// <summary>
        /// Gets or sets the load handler.
        /// </summary>
        /// <value>
        /// The load handler.
        /// </value>
        private LoadHandler LoadHandler { get; set; }
        #endregion
        #region RequestHandler
        /// <summary>
        /// Gets or sets the request handler.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        private RequestHandler RequestHandler { get; set; }
        #endregion
        #region KeyboardHandler
        /// <summary>
        /// Gets or sets the keyboard handler.
        /// </summary>
        /// <value>
        /// The keyboard handler.
        /// </value>
        private KeyboardHandler KeyboardHandler { get; set; }
        #endregion

        #region V8NativeBrowserHandler
        /// <summary>
        /// Gets or sets the v8 native browser handler.
        /// </summary>
        /// <value>
        /// The v8 native browser handler.
        /// </value>
        private V8NativeBrowserHandler V8NativeBrowserHandler { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        public Client()
            : base()
        {
            LifeSpanHandler = new LifeSpanHandler();
            DisplayHandler = new DisplayHandler();
            LoadHandler = new LoadHandler();
            RequestHandler = new RequestHandler();
            KeyboardHandler = new KeyboardHandler();

            // set events
            LifeSpanHandler.BrowserCreated += (sender, e) => {
                if (BrowserCreated != null)
                    BrowserCreated(this, e);
            };

            // native calls
            V8NativeBrowserHandler = new V8NativeBrowserHandler(NativeFunctionAttribute.GetMethods<Client, Func<string, object>>(this));
        }

        #endregion
        #region Methods
        #region Protected

        #region GetLifeSpanHandler
        /// <summary>
        /// Return the handler for browser life span events.
        /// </summary>
        /// <returns></returns>
        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return LifeSpanHandler;
        }
        #endregion
        #region GetDisplayHandler
        /// <summary>
        /// Return the handler for browser display state events.
        /// </summary>
        /// <returns></returns>
        protected override CefDisplayHandler GetDisplayHandler()
        {
            return DisplayHandler;
        }
        #endregion
        #region GetLoadHandler
        /// <summary>
        /// Return the handler for browser load status events.
        /// </summary>
        /// <returns></returns>
        protected override CefLoadHandler GetLoadHandler()
        {
            return LoadHandler;
        }
        #endregion
        #region GetRequestHandler
        /// <summary>
        /// Return the handler for browser request events.
        /// </summary>
        /// <returns></returns>
        protected override CefRequestHandler GetRequestHandler()
        {
            return RequestHandler;
        }
        #endregion
        #region GetKeyboardHandler
        /// <summary>
        /// Return the handler for keyboard events.
        /// </summary>
        /// <returns></returns>
        protected override CefKeyboardHandler GetKeyboardHandler()
        {
            return KeyboardHandler;
        }
        #endregion
        #region OnProcessMessageReceived
        /// <summary>
        /// Called when a new message is received from a different process. Return true
        /// if the message was handled or false otherwise. Do not keep a reference to
        /// or attempt to access the message outside of this callback.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="sourceProcess"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            return V8NativeBrowserHandler.ProcessMessage(browser, sourceProcess, message) ||
                BaseMainApplication.Current.BrowserMessageRouter.OnProcessMessageReceived(browser, sourceProcess, message);
        }
        #endregion

        #endregion
        #region Private

        #region Digest
        /// <summary>
        /// Digests the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        [NativeFunction]
        private object Digest(string json)
        {
            var controllerChanges = JsonConvert.DeserializeObject<List<ControllerChange>>(json);

            BaseMainApplication.Current.Window.Digest(controllerChanges);

            return null;
        }
        #endregion
        #region GetControllerNames
        /// <summary>
        /// Gets the controller names.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "json", Justification = "It has to match to the delegate.")]
        [NativeFunction]
        private object GetControllerNames(string json)
        {
            return BaseMainApplication.Current.ControllerProvider.ControllerTypes
                .Select(c => c.Name).ToList();
        }
        #endregion
        #region CreateController
        /// <summary>
        /// Creates the controller.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        [NativeFunction]
        private object CreateController(string json)
        {
            var createController = JsonConvert.DeserializeAnonymousType(json, new { Name = string.Empty, Id = 0 });
            var controller = BaseMainApplication.Current.Window.CreateController(createController.Name, createController.Id);

            return controller.GetDescription();
        }
        #endregion
        #region DestroyController
        /// <summary>
        /// Destroys the controller.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        [NativeFunction]
        private object DestroyController(string json)
        {
            var controllerId = JsonConvert.DeserializeObject<int>(json);

            BaseMainApplication.Current.Window.DestroyController(controllerId);

            return null;
        }
        #endregion
        #region CallMethod
        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        [NativeFunction]
        private object CallMethod(string json)
        {
            var methodData = JsonConvert.DeserializeAnonymousType(json, new { Id = 0, Name = string.Empty, Args = new JArray() });

            return BaseMainApplication.Current.Window.CallMethod(methodData.Id, methodData.Name, methodData.Args);
        }
        #endregion

        #endregion
        #endregion
    }
}
