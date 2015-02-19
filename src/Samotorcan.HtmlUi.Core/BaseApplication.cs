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
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Samotorcan.HtmlUi.Core.Validation;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Base application.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class BaseApplication : IDisposable
    {
        #region Constants

        #region LogsDirectory
        /// <summary>
        /// The logs directory.
        /// </summary>
        internal const string LogsDirectory = "Logs";
        #endregion

        #endregion
        #region Properties
        #region Public

        #region Current
        /// <summary>
        /// Gets the current application.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static BaseApplication Current { get; private set; }
        #endregion
        #region Window
        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        public BaseWindow Window { get; protected set; }
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
                Argument.InvalidOperation(Window.IsBrowserCreated, "EnableD3D11 can only be changed before the window is created.");

                _enableD3D11 = value;
            }
        }
        #endregion
        #region IsRunning
        /// <summary>
        /// Gets or sets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; private set; }
        #endregion
        #region IsMainThread
        /// <summary>
        /// Gets a value indicating whether current thread is main thread.
        /// </summary>
        /// <value>
        /// <c>true</c> if current thread is main thread; otherwise, <c>false</c>.
        /// </value>
        public bool IsMainThread
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId == ThreadId;
            }
        }
        #endregion
        #region ViewProvider
        private IViewProvider _viewProvider;
        /// <summary>
        /// Gets or sets the view provider.
        /// </summary>
        /// <value>
        /// The view provider.
        /// </value>
        public IViewProvider ViewProvider
        {
            get
            {
                return _viewProvider;
            }
            set
            {
                Argument.Null(value, "value");

                _viewProvider = value;
            }
        }
        #endregion

        #region RequestHostname
        private string _requestHostname;
        /// <summary>
        /// Gets or sets the request hostname.
        /// </summary>
        /// <value>
        /// The request hostname.
        /// </value>
        /// <exception cref="System.ArgumentException">Invalid request hostname.;RequestHostname</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Hostname", Justification = "Hostname is better.")]
        public string RequestHostname
        {
            get
            {
                return _requestHostname;
            }
            set
            {
                Argument.Null(value, "value");
                Argument.InvalidArgument(Uri.CheckHostName(value) != UriHostNameType.Dns, "Invalid request hostname.", "value");

                _requestHostname = value;
            }
        }
        #endregion
        #region RequestPort
        private int _requestPort;
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        /// <exception cref="System.ArgumentException">Invalid request port.;Port</exception>
        public int RequestPort
        {
            get
            {
                return _requestPort;
            }
            set
            {
                Argument.InvalidArgument(value < 0 || value > 65535, "Invalid request port.", "value");

                _requestPort = value;
            }
        }
        #endregion
        #region RequestViewPath
        private string _requestViewPath;
        /// <summary>
        /// Gets or sets the request view path.
        /// </summary>
        /// <value>
        /// The request view path.
        /// </value>
        /// <exception cref="System.ArgumentException">Invalid request view path;RequestViewsPath</exception>
        public string RequestViewPath
        {
            get
            {
                return _requestViewPath;
            }
            set
            {
                Argument.Null(value, "value");
                Argument.InvalidArgument(!PathUtility.IsFilePath(value), "Invalid request view path", "value");
                Argument.InvalidArgument(value.StartsWith("/") || value.EndsWith("/"), "RequestViewPath can't start or end with a slash.", "value");

                _requestViewPath = value;
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
        #region LogsDirectoryPath
        /// <summary>
        /// Gets or sets the logs directory path.
        /// </summary>
        /// <value>
        /// The logs directory path.
        /// </value>
        internal string LogsDirectoryPath { get; set; }
        #endregion

        #endregion
        #region Private

        #region ThreadId
        /// <summary>
        /// Gets or sets the thread identifier.
        /// </summary>
        /// <value>
        /// The thread identifier.
        /// </value>
        private int ThreadId { get; set; }
        #endregion
        #region InvokeQueue
        /// <summary>
        /// Gets or sets the invoke queue.
        /// </summary>
        /// <value>
        /// The invoke queue.
        /// </value>
        private BlockingCollection<Action> InvokeQueue { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApplication"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">You can only have one instance of Application at any given time.</exception>
        protected BaseApplication()
        {
            Argument.InvalidOperation(Current != null, "You can only have one instance of Application at any given time.");

            LogsDirectoryPath = PathUtility.NormalizedWorkingDirectory + "/" + LogsDirectory;
            EnsureLogsDirectory();

            ThreadId = Thread.CurrentThread.ManagedThreadId;
            Current = this;

            SynchronizationContext.SetSynchronizationContext(new HtmlUiSynchronizationContext());

            InitializeInvokeQueue();

            ViewProvider = new FileAssemblyViewProvider();
            RequestHostname = "localhost";
            RequestPort = 80;
            RequestViewPath = "Views";
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
            EnsureMainThread();

            Argument.InvalidOperation(IsRunning, "Application is already running.");

            IsRunning = true;

            try
            {
                InitializeCef();

                RegisterSchemes();
                RegisterMessageRouter();

                OnInitialize();
                RunMessageLoop();

                ShutdownInternal();
            }
            finally
            {
                IsRunning = false;
                InitializeInvokeQueue();
            }
        }
        #endregion
        #region InvokeOnMain
        /// <summary>
        /// Invokes the specified action on the main thread asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="System.InvalidOperationException">Application is shutting down.</exception>
        public Task<bool> InvokeOnMainAsync(Action action)
        {
            Argument.Null(action, "action");
            Argument.InvalidOperation(InvokeQueue.IsAddingCompleted, "Application is shutting down.");

            var taskCompletionSource = new TaskCompletionSource<bool>();

            InvokeQueue.Add(() =>
            {
                try
                {
                    action();

                    taskCompletionSource.SetResult(true);
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            });

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Invokes the specified action on the main thread synchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public bool InvokeOnMain(Action action)
        {
            return InvokeOnMainAsync(action).Result;
        }
        #endregion
        #region GetAbsoluteViewUrl
        /// <summary>
        /// Gets the absolute view URL.
        /// </summary>
        /// <param name="viewPath">The view path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">viewPath</exception>
        public string GetAbsoluteViewUrl(string viewPath)
        {
            Argument.NullOrEmpty(viewPath, "viewPath");

            return string.Format("http://{0}{1}/{2}/{3}",
                RequestHostname,
                RequestPort != 80 ? ":" + RequestPort : string.Empty,
                RequestViewPath,
                ViewProvider.GetUrlFromViewPath(viewPath));
        }
        #endregion
        #region GetViewPath
        /// <summary>
        /// Gets the view path.
        /// </summary>
        /// <param name="absoluteViewUrl">The absolute view URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">absoluteViewUrl</exception>
        public string GetViewPath(string absoluteViewUrl)
        {
            Argument.NullOrEmpty(absoluteViewUrl, "absoluteViewUrl");

            var match = Regex.Match(absoluteViewUrl, string.Format("^(http://)?{0}(:{1}){2}/{3}/(.+)$",
                Regex.Escape(RequestHostname),
                RequestPort,
                RequestPort == 80 ? "?" : "{1}",
                Regex.Escape(RequestViewPath)), RegexOptions.IgnoreCase);

            Argument.InvalidArgument(!match.Success, "Invalid url.", "absoluteViewUrl");

            return ViewProvider.GetViewPathFromUrl(match.Groups.OfType<Group>().Last().Value);
        }
        #endregion
        #region IsLocalUrl
        /// <summary>
        /// Determines whether the specified URL is local.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        public bool IsLocalUrl(string url)
        {
            Argument.NullOrEmpty(url, "url");

            return Regex.IsMatch(url, string.Format("^(http://)?{0}(:{1}){2}(/.*)?$",
                Regex.Escape(RequestHostname),
                RequestPort,
                RequestPort == 80 ? "?" : "{1}"), RegexOptions.IgnoreCase);
        }
        #endregion
        #region IsViewFileUrl
        /// <summary>
        /// Determines whether the specified URL is view file url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        public bool IsViewFileUrl(string url)
        {
            Argument.NullOrEmpty(url, "url");

            return Regex.IsMatch(url, string.Format("^(http://)?{0}(:{1}){2}/{3}/.+$",
                Regex.Escape(RequestHostname),
                RequestPort,
                RequestPort == 80 ? "?" : "{1}",
                RequestViewPath), RegexOptions.IgnoreCase);
        }
        #endregion

        #endregion
        #region Protected

        #region OnInitialize
        /// <summary>
        /// Called when initialize is triggered.
        /// </summary>
        protected virtual void OnInitialize() { }
        #endregion
        #region OnShutdown
        /// <summary>
        /// Called when shutdown is triggered.
        /// </summary>
        protected virtual void OnShutdown() { }
        #endregion

        #endregion
        #region internal

        #region EnsureMainThread
        /// <summary>
        /// Ensures that it's called from the main thread.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Must be called from the main thread.</exception>
        internal void EnsureMainThread()
        {
            Argument.InvalidOperation(Thread.CurrentThread.ManagedThreadId != ThreadId, "Must be called from the main thread.");
        }
        #endregion
        #region Shutdown
        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Application is not running.</exception>
        internal void Shutdown()
        {
            Argument.InvalidOperation(!IsRunning, "Application is not running.");

            InvokeQueue.CompleteAdding();
        }
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
                LogFile = LogsDirectoryPath + "/cef.log",
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
        }
        #endregion
        #region RunMessageLoop
        /// <summary>
        /// Runs the message loop.
        /// </summary>
        private void RunMessageLoop()
        {
            foreach (var action in InvokeQueue.GetConsumingEnumerable())
                action();

            InvokeQueue.Dispose();
            OnShutdown();
        }
        #endregion
        #region InitializeInvokeQueue
        /// <summary>
        /// Initializes the invoke queue.
        /// </summary>
        private void InitializeInvokeQueue()
        {
            if (InvokeQueue != null)
                InvokeQueue.Dispose();

            InvokeQueue = new BlockingCollection<Action>(new ConcurrentQueue<Action>(), 100);
        }
        #endregion
        #region EnsureLogsDirectory
        /// <summary>
        /// Ensures the logs directory.
        /// </summary>
        private void EnsureLogsDirectory()
        {
            if (!Directory.Exists(LogsDirectoryPath))
                Directory.CreateDirectory(LogsDirectoryPath);
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
            EnsureMainThread();

            if (!_disposed)
            {
                if (disposing)
                {
                    Current = null;

                    if (InvokeQueue != null)
                        InvokeQueue.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="BaseApplication"/> class.
        /// </summary>
        ~BaseApplication()
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
