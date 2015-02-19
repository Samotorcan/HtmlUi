using Samotorcan.HtmlUi.Core.Events;
using Samotorcan.HtmlUi.Core.Handlers.Browser;
using Samotorcan.HtmlUi.Core.Utilities;
using Samotorcan.HtmlUi.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Base window.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class BaseWindow : IDisposable
    {
        #region Constants

        #region DefaultView
        /// <summary>
        /// The default view.
        /// </summary>
        private const string DefaultView = "Index.html";
        #endregion

        #endregion
        #region Events

        #region BrowserCreated
        /// <summary>
        /// Occurs when browser is created.
        /// </summary>
        protected event EventHandler<BrowserCreatedEventArgs> BrowserCreated;
        #endregion

        #endregion
        #region Properties
        #region Public

        #region View
        private string _view;
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        /// <exception cref="System.InvalidOperationException">View can only be changed before the window is created.</exception>
        public string View
        {
            get
            {
                return _view;
            }
            set
            {
                BaseApplication.Current.EnsureMainThread();

                Argument.InvalidOperation(IsBrowserCreated, "View can only be changed before the window is created.");

                _view = value;
            }
        }
        #endregion
        #region Borderless
        private bool _borderless;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BaseWindow"/> is borderless.
        /// </summary>
        /// <value>
        ///   <c>true</c> if borderless; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Borderless
        {
            get
            {
                return _borderless;
            }
            set
            {
                BaseApplication.Current.EnsureMainThread();

                _borderless = value;
            }
        }
        #endregion

        #endregion
        #region Internal

        #region IsBrowserCreated
        /// <summary>
        /// Gets a value indicating whether browser is created.
        /// </summary>
        /// <value>
        /// <c>true</c> if browser is created; otherwise, <c>false</c>.
        /// </value>
        internal bool IsBrowserCreated { get; private set; }
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
        protected CefBrowser CefBrowser { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWindow"/> class.
        /// </summary>
        protected BaseWindow() { }

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
            BaseApplication.Current.EnsureMainThread();

            Argument.InvalidOperation(IsBrowserCreated, "Browser already created.");

            var cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsChild(handle, position);

            var cefClient = new DefaultCefClient();
            cefClient.BrowserCreated += (sender, e) =>
            {
                CefBrowser = e.CefBrowser;

                if (BrowserCreated != null)
                    BrowserCreated(this, e);
            };

            var cefSettings = new CefBrowserSettings();

            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefSettings, BaseApplication.Current.GetAbsoluteViewUrl(View));

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
            BaseApplication.Current.EnsureMainThread();

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
