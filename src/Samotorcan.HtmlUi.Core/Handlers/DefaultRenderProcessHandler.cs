using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Samotorcan.HtmlUi.Core.Handlers
{
    /// <summary>
    /// Default render process handler.
    /// </summary>
    [CLSCompliant(false)]
    public class DefaultRenderProcessHandler : CefRenderProcessHandler
    {
        #region Properties
        #region Private

        #region MessageRouter
        /// <summary>
        /// Gets or sets the message router.
        /// </summary>
        /// <value>
        /// The message router.
        /// </value>
        private CefMessageRouterRendererSide MessageRouter { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderProcessHandler"/> class.
        /// </summary>
        public DefaultRenderProcessHandler()
        {
            MessageRouter = new CefMessageRouterRendererSide(new CefMessageRouterConfig());
        }

        #endregion
        #region Methods
        #region Protected

        #region OnContextCreated
        /// <summary>
        /// Called when context created.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="context">The context.</param>
        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            MessageRouter.OnContextCreated(browser, frame, context);
        }
        #endregion
        #region OnContextReleased
        /// <summary>
        /// Called when context released.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="context">The context.</param>
        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            MessageRouter.OnContextReleased(browser, frame, context);
        }
        #endregion
        #region OnProcessMessageReceived
        /// <summary>
        /// Called when process message received.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="sourceProcess">The source process.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            return MessageRouter.OnProcessMessageReceived(browser, sourceProcess, message);
        }
        #endregion

        #endregion
        #endregion
    }
}
