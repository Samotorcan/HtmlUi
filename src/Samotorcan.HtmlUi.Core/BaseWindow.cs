﻿using Samotorcan.HtmlUi.Core.Events;
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
    /// Base window.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class BaseWindow : IDisposable
    {
        #region Events

        #region BrowserCreated
        /// <summary>
        /// Occurs when browser is created.
        /// </summary>
        protected event EventHandler<BrowserCreatedEventArgs> BrowserCreated;
        #endregion
        #region KeyPress
        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        protected event EventHandler<KeyPressEventArgs> KeyPress;
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

                if (IsBrowserCreated)
                    throw new InvalidOperationException("View can only be changed before the window is created.");

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
        #region Controllers
        /// <summary>
        /// Gets or sets the controllers.
        /// </summary>
        /// <value>
        /// The controllers.
        /// </value>
        internal List<Controller> Controllers { get; set; }
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
        protected BaseWindow()
        {
            View = "/Views/Index.html";
            Controllers = new List<Controller>();
        }

        #endregion
        #region Methods
        #region Internal

        #region TriggerKeyPress
        /// <summary>
        /// Triggers the key press.
        /// </summary>
        /// <param name="nativeKeyCode">The native key code.</param>
        internal void TriggerKeyPress(int nativeKeyCode)
        {
            if (KeyPress != null)
                KeyPress(this, new KeyPressEventArgs(nativeKeyCode));
        }
        #endregion
        #region CreateControllers
        /// <summary>
        /// Creates the controllers.
        /// </summary>
        internal void CreateControllers()
        {
            var controllerProvider = BaseApplication.Current.ControllerProvider;
            var controllerTypes = controllerProvider.GetControllerTypes();
            var createdControllers = new List<Controller>();

            try
            {
                foreach (var controllerType in controllerTypes)
                {
                    createdControllers.Add(controllerProvider.CreateController(controllerType.Name));
                }
            }
            catch (Exception)
            {
                foreach (var createdController in createdControllers)
                    createdController.Dispose();

                throw;
            }

            // dispose current controllers
            foreach (var controller in Controllers)
                controller.Dispose();

            // save created controllers
            Controllers = createdControllers;
        }
        #endregion

        #endregion
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

            if (IsBrowserCreated)
                throw new InvalidOperationException("Browser already created.");

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

            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefSettings, BaseApplication.Current.GetAbsoluteContentUrl(View));

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
                    // CEF browser
                    if (CefBrowser != null)
                    {
                        var host = CefBrowser.GetHost();

                        if (host != null)
                        {
                            host.CloseBrowser(true);
                            host.Dispose();
                        }

                        CefBrowser.Dispose();
                    }
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
