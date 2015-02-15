using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Handlers.Scheme;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;
using Samotorcan.HtmlUi.Core.Handlers;
using System.Diagnostics;
using System.Threading;
using Samotorcan.HtmlUi.Core.Utilities;
using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Core.Exceptions;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// The main application.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Application : IDisposable
    {
        #region Properties
        #region Public

        #region Current
        /// <summary>
        /// Gets the current application.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static Application Current { get; private set; }
        #endregion
        #region Window
        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        public Window Window { get; private set; }
        #endregion
        #region EnableD3D11
        private bool _enableD3D11;
        /// <summary>
        /// Gets or sets a value indicating whether d3d11 is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if d3d11 is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnableD3D11
        {
            get
            {
                return _enableD3D11;
            }
            set
            {
                if (Window.IsBrowserCreated)
                    throw new InvalidOperationException("EnableD3D11 can only be changed before the window is created.");

                _enableD3D11 = value;
            }
        }
        #endregion

        #endregion
        #region Internal

        #region BrowserMessageRouter
        /// <summary>
        /// Gets or sets the browser message router.
        /// </summary>
        /// <value>
        /// The browser message router.
        /// </value>
        internal CefMessageRouterBrowserSide BrowserMessageRouter { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">You can only have one instance of Application at any given time.</exception>
        public Application()
        {
            if (Current != null)
                throw new InvalidOperationException("You can only have one instance of Application at any given time.");

            Current = this;

            Initialize();

            Window = CreateWindow();
        }

        #endregion
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        public void Run()
        {
            try
            {
                InitializeCef();

                RegisterSchemes();
                RegisterMessageRouter();

                RunMessageLoop();

                ShutdownInternal();
            }
            catch (Exception e)
            {
                GeneralLog.Warn("Exception while trying to run the application.", e);
                throw;
            }
        }
        #endregion

        #endregion
        #region Protected

        #region OnShutdown
        /// <summary>
        /// Called when the application is shutdown.
        /// </summary>
        protected virtual void OnShutdown() { }
        #endregion
        #region OnInitialize
        /// <summary>
        /// Called when the application is initialized.
        /// </summary>
        protected virtual void OnInitialize() { }
        #endregion
        #region RunMessageLoop
        /// <summary>
        /// Runs the message loop.
        /// </summary>
        protected abstract void RunMessageLoop();
        #endregion
        #region CreateWindow
        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <returns></returns>
        protected abstract Window CreateWindow();
        #endregion

        #endregion
        #region Private

        #region InitializeCef
        /// <summary>
        /// Initializes the cef.
        /// </summary>
        /// <exception cref="Samotorcan.HtmlUi.Core.Exceptions.CefProcessException"></exception>
        private void InitializeCef()
        {
            CefRuntime.Load();

            var cefSettings = new CefSettings
            {
                MultiThreadedMessageLoop = true,
                SingleProcess = false,
                LogSeverity = CefLogSeverity.Warning,
                LogFile = "cef.log",
                ResourcesDirPath = PathUtility.WorkingDirectory,
                RemoteDebuggingPort = 20480,
                NoSandbox = true,
                CommandLineArgsDisabled = true
            };

            // arguments
            var arguments = new List<string>();
            if (!EnableD3D11)
                arguments.Add(CefArgument.DisableD3D11.Value);

            // initialize
            var mainArgs = new CefMainArgs(arguments.ToArray());
            var app = new DefaultCefApp();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
            if (exitCode != -1)
                throw new CefProcessException(exitCode);

            CefRuntime.Initialize(mainArgs, cefSettings, app, IntPtr.Zero);
        }
        #endregion
        #region RegisterSchemes
        /// <summary>
        /// Registers the schemes.
        /// </summary>
        private void RegisterSchemes()
        {
            // TODO | Samotorcan: Temp.
            CefRuntime.RegisterSchemeHandlerFactory("http", "test.test.test", new DefaultAppSchemeHandlerFactory());
        }
        #endregion
        #region RegisterMessageRouter
        /// <summary>
        /// Registers the message router.
        /// </summary>
        private void RegisterMessageRouter()
        {
            BrowserMessageRouter = new CefMessageRouterBrowserSide(new CefMessageRouterConfig());
        }
        #endregion
        #region ShutdownInternal
        /// <summary>
        /// Shutdowns cef.
        /// </summary>
        private void ShutdownInternal()
        {
            CefRuntime.Shutdown();

            OnShutdown();
        }
        #endregion
        #region Initialize
        /// <summary>
        /// Initialize.
        /// </summary>
        private void Initialize()
        {
            OnInitialize();
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
                    Current = null;
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Application"/> class.
        /// </summary>
        ~Application()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
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
