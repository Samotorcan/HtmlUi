using Samotorcan.HtmlUi.Core.Renderer.Handlers;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Renderer
{
    /// <summary>
    /// Developer tools CEF client.
    /// </summary>
    internal class DeveloperToolsClient : CefClient
    {
        #region Properties
        #region Private

        #region RequestHandler
        /// <summary>
        /// Gets or sets the request handler.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        private CefRequestHandler RequestHandler { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeveloperToolsClient"/> class.
        /// </summary>
        public DeveloperToolsClient()
            : base()
        {
            RequestHandler = new DeveloperToolsRequestHandler();
        }

        #endregion
        #region Methods
        #region Protected

        #region GetRequestHandler
        /// <summary>
        /// Gets the request handler.
        /// </summary>
        /// <returns></returns>
        protected override CefRequestHandler GetRequestHandler()
        {
            return RequestHandler;
        }
        #endregion

        #endregion
        #endregion
    }
}
