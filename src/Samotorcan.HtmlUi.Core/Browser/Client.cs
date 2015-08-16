using Samotorcan.HtmlUi.Core.Browser.Handlers;
using Samotorcan.HtmlUi.Core.Events;
using System;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Browser
{
    /// <summary>
    /// Client.
    /// </summary>
    internal class Client : CefClient
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

        #region NativeMessageHandler
        /// <summary>
        /// Gets or sets the native message handler.
        /// </summary>
        /// <value>
        /// The native message handler.
        /// </value>
        private NativeMessageHandler NativeMessageHandler { get; set; }
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
            LifeSpanHandler.BrowserCreated += (sender, e) =>
            {
                if (BrowserCreated != null)
                    BrowserCreated(this, e);
            };

            // native calls
            NativeMessageHandler = new NativeMessageHandler();
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

            return NativeMessageHandler.ProcessMessage(browser, sourceProcess, message);
        }
        #endregion

        #endregion
        #endregion
    }
}
