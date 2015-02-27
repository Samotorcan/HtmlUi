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

        #region CefLifeSpanHandler
        /// <summary>
        /// Gets or sets the cef life span handler.
        /// </summary>
        /// <value>
        /// The cef life span handler.
        /// </value>
        private LifeSpanHandler CefLifeSpanHandler { get; set; }
        #endregion
        #region CefDisplayHandler
        /// <summary>
        /// Gets or sets the cef display handler.
        /// </summary>
        /// <value>
        /// The cef display handler.
        /// </value>
        private DisplayHandler CefDisplayHandler { get; set; }
        #endregion
        #region CefLoadHandler
        /// <summary>
        /// Gets or sets the cef load handler.
        /// </summary>
        /// <value>
        /// The cef load handler.
        /// </value>
        private LoadHandler CefLoadHandler { get; set; }
        #endregion
        #region CefRequestHandler
        /// <summary>
        /// Gets or sets the cef request handler.
        /// </summary>
        /// <value>
        /// The cef request handler.
        /// </value>
        private RequestHandler CefRequestHandler { get; set; }
        #endregion
        #region CefKeyboardHandler
        /// <summary>
        /// Gets or sets the cef keyboard handler.
        /// </summary>
        /// <value>
        /// The cef keyboard handler.
        /// </value>
        private KeyboardHandler CefKeyboardHandler { get; set; }
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
            CefLifeSpanHandler = new LifeSpanHandler();
            CefDisplayHandler = new DisplayHandler();
            CefLoadHandler = new LoadHandler();
            CefRequestHandler = new RequestHandler();
            CefKeyboardHandler = new KeyboardHandler();

            // set events
            CefLifeSpanHandler.BrowserCreated += (sender, e) => {
                if (BrowserCreated != null)
                    BrowserCreated(this, e);
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
            return CefLifeSpanHandler;
        }
        #endregion
        #region GetDisplayHandler
        /// <summary>
        /// Return the handler for browser display state events.
        /// </summary>
        /// <returns></returns>
        protected override CefDisplayHandler GetDisplayHandler()
        {
            return CefDisplayHandler;
        }
        #endregion
        #region GetLoadHandler
        /// <summary>
        /// Return the handler for browser load status events.
        /// </summary>
        /// <returns></returns>
        protected override CefLoadHandler GetLoadHandler()
        {
            return CefLoadHandler;
        }
        #endregion
        #region GetRequestHandler
        /// <summary>
        /// Return the handler for browser request events.
        /// </summary>
        /// <returns></returns>
        protected override CefRequestHandler GetRequestHandler()
        {
            return CefRequestHandler;
        }
        #endregion
        #region GetKeyboardHandler
        /// <summary>
        /// Return the handler for keyboard events.
        /// </summary>
        /// <returns></returns>
        protected override CefKeyboardHandler GetKeyboardHandler()
        {
            return CefKeyboardHandler;
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

            // digest
            if (processMessage.Name == "Digest")
            {
                var message = MessageUtility.DeserializeMessage<List<ControllerChange>>(processMessage);

                BaseMainApplication.Current.InvokeOnMainAsync(() =>
                {
                    BaseMainApplication.Current.Digest(message.Data);

                    // callback
                    if (message.CallbackId != null)
                    {
                        CefUtility.ExecuteTask(CefThreadId.UI, () =>
                        {
                            MessageUtility.SendMessage(browser, "DigestCallback", message.CallbackId);
                        });
                    }
                });

                return true;
            }

            return BaseMainApplication.Current.BrowserMessageRouter.OnProcessMessageReceived(browser, sourceProcess, processMessage);
        }
        #endregion

        #endregion
        #endregion
    }
}
