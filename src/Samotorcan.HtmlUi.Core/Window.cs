using Samotorcan.HtmlUi.Core.Handlers.Browser;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Window.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Window : IDisposable
    {
        #region Constants

        #region DefaultUrl
        /// <summary>
        /// The default URL.
        /// </summary>
        private const string DefaultUrl = "http://localhost/views/index.html";
        #endregion

        #endregion
        #region Events

        #region BrowserCreated
        /// <summary>
        /// Occurs when browser is created.
        /// </summary>
        protected event EventHandler<CefBrowser> BrowserCreated;
        #endregion

        #endregion
        #region Properties
        #region Public

        #region Url
        private string _url;
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (IsBrowserCreated)
                    throw new InvalidOperationException("Url can only be changed before the window is created.");

                _url = value;
            }
        }
        #endregion
        #region Borderless
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Window"/> is borderless.
        /// </summary>
        /// <value>
        ///   <c>true</c> if borderless; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Borderless { get; set; }
        #endregion

        #endregion
        #region Protected

        #region CefBrowser
        /// <summary>
        /// Gets or sets the cef browser.
        /// </summary>
        /// <value>
        /// The cef browser.
        /// </value>
        protected CefBrowser CefBrowser { get; private set; }
        #endregion
        #region IsBrowserCreated
        /// <summary>
        /// Gets a value indicating whether browser is created.
        /// </summary>
        /// <value>
        /// <c>true</c> if browser is created; otherwise, <c>false</c>.
        /// </value>
        internal protected bool IsBrowserCreated { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window(string url)
            : base()
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            if (!UriUtility.IsAbsoluteUri(url))
                throw new ArgumentException("Invalid url.", "url");

            Url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window()
            : this(DefaultUrl) { }

        #endregion
        #region Methods
        #region Protected

        #region CreateBrowser
        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="position">The position.</param>
        protected void CreateBrowser(IntPtr handle, CefRectangle position)
        {
            var cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsChild(handle, position);

            var cefClient = new DefaultCefClient();
            cefClient.BrowserCreated += (sender, browser) =>
            {
                CefBrowser = browser;

                if (BrowserCreated != null)
                    BrowserCreated(this, browser);
            };

            var cefSettings = new CefBrowserSettings();

            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefSettings, Url);

            IsBrowserCreated = true;
        }
        #endregion

        #endregion
        #endregion

        #region IDisposable

        /// <summary>
        /// Was dispose already called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    var host = CefBrowser.GetHost();

                    host.CloseBrowser(true);
                    host.Dispose();

                    CefBrowser.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
