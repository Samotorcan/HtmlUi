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
using System.Collections.ObjectModel;
using Newtonsoft.Json;

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
        #region ContentProvider
        private IContentProvider _contentProvider;
        /// <summary>
        /// Gets or sets the content provider.
        /// </summary>
        /// <value>
        /// The content provider.
        /// </value>
        public IContentProvider ContentProvider
        {
            get
            {
                return _contentProvider;
            }
            set
            {
                EnsureMainThread();

                _contentProvider = value;
            }
        }
        #endregion
        #region ControllerProvider
        private IControllerProvider _controllerProvider;
        /// <summary>
        /// Gets or sets the controller provider.
        /// </summary>
        /// <value>
        /// The controller provider.
        /// </value>
        public IControllerProvider ControllerProvider
        {
            get
            {
                return _controllerProvider;
            }
            set
            {
                EnsureMainThread();

                _controllerProvider = value;
            }
        }
        #endregion
        #region HtmlFileExtensions
        /// <summary>
        /// Gets the HTML file extensions.
        /// </summary>
        /// <value>
        /// The HTML file extensions.
        /// </value>
        public Collection<string> HtmlFileExtensions { get; private set; }
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
        #region ExitException
        /// <summary>
        /// Gets or sets the exit exception.
        /// </summary>
        /// <value>
        /// The exit exception.
        /// </value>
        private Exception ExitException { get; set; }
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

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            LogsDirectoryPath = PathUtility.NormalizedWorkingDirectory + "/" + LogsDirectory;
            EnsureLogsDirectory();

            ThreadId = Thread.CurrentThread.ManagedThreadId;
            Current = this;

            SynchronizationContext.SetSynchronizationContext(new HtmlUiSynchronizationContext());

            InitializeInvokeQueue();

            ContentProvider = new FileAssemblyContentProvider();
            ControllerProvider = new AssemblyControllerProvider();
            RequestHostname = "localhost";
            RequestPort = 80;

            HtmlFileExtensions = new Collection<string> { "html" };
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

            // exit the run method with the exception
            if (ExitException != null)
            {
                Dispose();
                throw ExitException;
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
            Argument.Null(action, "action");

            if (!IsMainThread)
                return InvokeOnMainAsync(action).Result;

            action();

            return true;
        }
        #endregion
        #region GetAbsoluteContentUrl
        /// <summary>
        /// Gets the absolute content URL.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        /// <returns></returns>
        public string GetAbsoluteContentUrl(string contentPath)
        {
            Argument.NullOrEmpty(contentPath, "contentPath");

            return string.Format("http://{0}{1}/{2}",
                RequestHostname,
                RequestPort != 80 ? ":" + RequestPort : string.Empty,
                ContentProvider.GetUrlFromContentPath(contentPath).TrimStart('/'));
        }
        #endregion
        #region GetContentPath
        /// <summary>
        /// Gets the content path.
        /// </summary>
        /// <param name="absoluteContentUrl">The absolute content URL.</param>
        /// <returns></returns>
        public string GetContentPath(string absoluteContentUrl)
        {
            Argument.NullOrEmpty(absoluteContentUrl, "absoluteContentUrl");

            var match = Regex.Match(absoluteContentUrl,
                string.Format("^(http://)?{0}(:{1}){2}/(.+)$",
                    Regex.Escape(RequestHostname),
                    RequestPort,
                    RequestPort == 80 ? "?" : "{1}"),
                RegexOptions.IgnoreCase);

            Argument.InvalidArgument(!match.Success, "Invalid url.", "absoluteContentUrl");

            return ContentProvider.GetContentPathFromUrl(match.Groups.OfType<Group>().Last().Value);
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
            EnsureMainThread();

            Argument.InvalidOperation(!IsRunning, "Application is not running.");

            Window.Dispose();
            InvokeQueue.CompleteAdding();
        }
        #endregion
        #region ShutdownWithException
        /// <summary>
        /// Shutdowns with the exception.
        /// </summary>
        /// <param name="e">The e.</param>
        internal void ShutdownWithException(Exception e)
        {
            Shutdown();

            ExitException = e;
        }
        #endregion
        #region Digest
        /// <summary>
        /// Digest call. Updates the controllers.
        /// </summary>
        /// <param name="controllerChanges">The controller changes.</param>
        internal void Digest(IEnumerable<ControllerChange> controllerChanges)
        {
            EnsureMainThread();

            GeneralLog.Info(string.Format("Digest call: {0}", JsonConvert.SerializeObject(controllerChanges))); // TODO: add more info logs

            foreach (var controllerChange in controllerChanges)
            {
                // TODO: implement
            }
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
        #region CurrentDomain_UnhandledException
        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GeneralLog.Error("Unhandled exception.", e.ExceptionObject as Exception);
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

                    AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
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
