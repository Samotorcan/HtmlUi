using Newtonsoft.Json;
using Samotorcan.HtmlUi.Core.Browser.Handlers;
using Samotorcan.HtmlUi.Core.Events;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region ProcessMessages
        /// <summary>
        /// Gets or sets the process messages.
        /// </summary>
        /// <value>
        /// The process messages.
        /// </value>
        private Dictionary<string, Action<CefBrowser, CefProcessId, CefProcessMessage>> ProcessMessages { get; set; }
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

            ProcessMessages = new Dictionary<string, Action<CefBrowser, CefProcessId, CefProcessMessage>>
            {
                { "Digest", Digest },
                { "ControllerNames", ControllerNames },
                { "CreateController", CreateController },
                { "DestroyController", DestroyController },
            };
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
        /// <param name="processMessage"></param>
        /// <returns></returns>
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            if (processMessage == null)
                throw new ArgumentNullException("message");

            if (ProcessMessages.ContainsKey(processMessage.Name))
            {
                ProcessMessages[processMessage.Name](browser, sourceProcess, processMessage);

                return true;
            }

            return BaseMainApplication.Current.BrowserMessageRouter.OnProcessMessageReceived(browser, sourceProcess, processMessage);
        }
        #endregion

        #endregion
        #region Private

        #region Digest
        /// <summary>
        /// Digest call.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="sourceProcess">The source process.</param>
        /// <param name="processMessage">The process message.</param>
        private void Digest(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var message = MessageUtility.DeserializeMessage<List<ControllerChange>>(processMessage);

            BaseMainApplication.Current.InvokeOnMainAsync(() =>
            {
                BaseMainApplication.Current.Window.Digest(message.Data);

                // callback
                if (message.CallbackId != null)
                {
                    MessageUtility.SendMessage(browser, "DigestCallback", message.CallbackId);
                }
            });
        }
        #endregion
        #region ControllerNames
        /// <summary>
        /// Controller names call.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="sourceProcess">The source process.</param>
        /// <param name="processMessage">The process message.</param>
        private void ControllerNames(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var message = MessageUtility.DeserializeMessage(processMessage);

            BaseMainApplication.Current.InvokeOnMainAsync(() =>
            {
                // callback
                if (message.CallbackId != null)
                {
                    var controllerTypes = BaseMainApplication.Current.ControllerProvider.GetControllerTypes();
                    var controllerNames = controllerTypes.Select(c => c.Name).ToList();

                    MessageUtility.SendMessage(browser, "ControllerNamesCallback", message.CallbackId, controllerNames);
                }
            });
        }
        #endregion
        #region CreateController
        /// <summary>
        /// Create controller call.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="sourceProcess">The source process.</param>
        /// <param name="processMessage">The process message.</param>
        private void CreateController(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var message = MessageUtility.DeserializeAnonymousMessage(processMessage, new { Name = string.Empty, Id = 0 });

            BaseMainApplication.Current.InvokeOnMainAsync(() =>
            {
                var controller = BaseMainApplication.Current.Window.CreateController(message.Data.Name, message.Data.Id);

                // callback
                if (message.CallbackId != null)
                    MessageUtility.SendMessage(browser, "CreateControllerCallback", message.CallbackId, controller.GetDescription());
            });
        }
        #endregion
        #region DestroyController
        /// <summary>
        /// Destroy controller call.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="sourceProcess">The source process.</param>
        /// <param name="processMessage">The process message.</param>
        private void DestroyController(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage processMessage)
        {
            var message = MessageUtility.DeserializeMessage<int>(processMessage);

            BaseMainApplication.Current.InvokeOnMainAsync(() =>
            {
                var controller = BaseMainApplication.Current.Window.DestroyController(message.Data);

                // callback
                if (message.CallbackId != null)
                    MessageUtility.SendMessage(browser, "DestroyControllerCallback", message.CallbackId);
            });
        }
        #endregion

        #endregion
        #endregion
    }
}
