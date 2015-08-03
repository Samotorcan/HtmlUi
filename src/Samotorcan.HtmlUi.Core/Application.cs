using Samotorcan.HtmlUi.Core.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xilium.CefGlue;
using System.Threading;
using Samotorcan.HtmlUi.Core.Utilities;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Samotorcan.HtmlUi.Core.Browser;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Application.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Application : BaseApplication
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
        public static new Application Current
        {
            get
            {
                return (Application)BaseApplication.Current;
            }
        }
        #endregion
        #region Window
        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        public Window Window { get; protected set; }
        #endregion
        #region D3D11Enabled
        /// <summary>
        /// Gets a value indicating whether D3D11 is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if D3D11 is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool D3D11Enabled { get; private set; }
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
        #region MimeTypes
        /// <summary>
        /// Gets the MIME types.
        /// </summary>
        /// <value>
        /// The MIME types.
        /// </value>
        public Dictionary<string, string> MimeTypes { get; private set; }
        #endregion
        #region SyncMaxDepth
        /// <summary>
        /// Gets or sets the synchronize maximum depth.
        /// </summary>
        /// <value>
        /// The synchronize maximum depth.
        /// </value>
        public int SyncMaxDepth { get; set; }
        #endregion
        #region RemoteDebuggingPort
        /// <summary>
        /// Gets the remote debugging port.
        /// </summary>
        /// <value>
        /// The remote debugging port.
        /// </value>
        public int RemoteDebuggingPort { get; private set; }
        #endregion
        #region CommandLineArgsEnabled
        /// <summary>
        /// Gets or sets a value indicating whether command line arguments are enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if command line arguments are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CommandLineArgsEnabled { get; private set; }
        #endregion
        #region ChromeViewsEnabled
        /// <summary>
        /// Gets or sets a value indicating whether chrome views are enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if chrome views are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool ChromeViewsEnabled { get; set; }
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
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("value");

                if (Uri.CheckHostName(value) != UriHostNameType.Dns)
                    throw new ArgumentException("Invalid request hostname.", "value");

                _requestHostname = value;
            }
        }
        #endregion

        #endregion
        #region Internal

        #region NativeRequestPort
        private int _nativeRequestPort;
        /// <summary>
        /// Gets or sets the native request port.
        /// </summary>
        /// <value>
        /// The native request port.
        /// </value>
        /// <exception cref="System.ArgumentException">Invalid port.;Port</exception>
        internal int NativeRequestPort
        {
            get
            {
                return _nativeRequestPort;
            }
            set
            {
                if (value < 0 || value > 65535)
                    throw new ArgumentException("Invalid port.", "value");

                _nativeRequestPort = value;
            }
        }
        #endregion
        #region NativeRequestUrl
        /// <summary>
        /// Gets the native request URL.
        /// </summary>
        /// <value>
        /// The native request URL.
        /// </value>
        public string NativeRequestUrl
        {
            get
            {
                return string.Format("http://{0}:{1}/", RequestHostname, NativeRequestPort);
            }
        }
        #endregion
        #region IsCefThread
        /// <summary>
        /// Gets a value indicating whether this instance is cef thread.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is cef thread; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCefThread
        {
            get
            {
                return IsMultiThreadedMessageLoop ? IsMainThread
                    : CefThread != null ? CefThread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId
                    : false;
            }
        }
        #endregion
        #region IsMultiThreadedMessageLoop
        /// <summary>
        /// Gets a value indicating whether this instance is multi threaded message loop.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multi threaded message loop; otherwise, <c>false</c>.
        /// </value>
        internal bool IsMultiThreadedMessageLoop
        {
            get
            {
                return CefRuntime.Platform == CefRuntimePlatform.Windows;
            }
        }
        #endregion

        #endregion
        #region Private

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
        #region CefThread
        /// <summary>
        /// Gets or sets the cef thread.
        /// </summary>
        /// <value>
        /// The cef thread.
        /// </value>
        private Thread CefThread { get; set; }
        #endregion
        #region SingleThreadedMessageLoopStoped
        /// <summary>
        /// Gets or sets the single threaded message loop stoped.
        /// </summary>
        /// <value>
        /// The single threaded message loop stoped.
        /// </value>
        private AutoResetEvent SingleThreadedMessageLoopStoped { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.InvalidOperationException">Application must be run on main application process.</exception>
        protected Application(ApplicationSettings settings)
            : base(settings)
        {
            if (HtmlUiRuntime.ApplicationType != ApplicationType.Application)
                throw new InvalidOperationException("Application must be run on main application process.");

            SynchronizationContext.SetSynchronizationContext(new HtmlUiSynchronizationContext());

            InitializeInvokeQueue();

            ContentProvider = new FileAssemblyContentProvider();
            ControllerProvider = new AssemblyControllerProvider();
            RequestHostname = "localhost";
            NativeRequestPort = 16556;
            D3D11Enabled = settings.D3D11Enabled;
            RemoteDebuggingPort = settings.RemoteDebuggingPort ?? 0;
            CommandLineArgsEnabled = settings.CommandLineArgsEnabled;
            ChromeViewsEnabled = settings.ChromeViewsEnabled;

            MimeTypes = GetDefaultMimeTypes();
            SyncMaxDepth = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class with default settings.
        /// </summary>
        protected Application()
            : this(new ApplicationSettings()) { }

        #endregion
        #region Methods
        #region Public

        #region InvokeOnMain
        /// <summary>
        /// Invokes the specified action on the main thread asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="System.InvalidOperationException">Application is shutting down.</exception>
        public Task InvokeOnMainAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (InvokeQueue.IsAddingCompleted)
                throw new InvalidOperationException("Application is shutting down.");

            Logger.Debug("Invoke on main called.");

            var taskCompletionSource = new TaskCompletionSource<object>();

            InvokeQueue.Add(() =>
            {
                try
                {
                    action();
                    Window.SyncControllerChangesToClient(); // TODO: try to fix this

                    taskCompletionSource.SetResult(null);
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
        public void InvokeOnMain(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (!IsMainThread)
            {
                InvokeOnMainAsync(action).Wait();
            }
            else
            {
                action();
                Window.SyncControllerChangesToClient();
            }
        }
        #endregion
        #region GetContentUrl
        /// <summary>
        /// Gets the content URL.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        /// <returns></returns>
        public string GetContentUrl(string contentPath)
        {
            if (string.IsNullOrWhiteSpace(contentPath))
                throw new ArgumentNullException("contentPath");

            if (ChromeViewsEnabled && Regex.IsMatch(contentPath, "^chrome://about/?$"))
                return "chrome://about";

            return string.Format("http://{0}/{1}",
                RequestHostname,
                ContentProvider.GetUrlFromContentPath(contentPath).TrimStart('/'));
        }
        #endregion
        #region GetContentPath
        /// <summary>
        /// Gets the content path.
        /// </summary>
        /// <param name="contentUrl">The content URL.</param>
        /// <returns></returns>
        public string GetContentPath(string contentUrl)
        {
            if (string.IsNullOrWhiteSpace(contentUrl))
                throw new ArgumentNullException("contentUrl");

            var match = Regex.Match(contentUrl,
                string.Format("^(http://)?{0}(:80)?/(.+)$",
                    Regex.Escape(RequestHostname)),
                RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new ArgumentException("Invalid url.", "contentUrl");

            return ContentProvider.GetContentPathFromUrl(match.Groups.OfType<Group>().Last().Value);
        }
        #endregion
        #region IsContentUrl
        /// <summary>
        /// Determines whether the specified URL is content.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        public bool IsContentUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            return Regex.IsMatch(url, string.Format("^(http://)?{0}(:80)?(/.*)?$",
                    Regex.Escape(RequestHostname)),
                RegexOptions.IgnoreCase);
        }
        #endregion

        #endregion
        #region internal

        #region Shutdown
        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Application is not running.</exception>
        internal void Shutdown()
        {
            EnsureMainThread();

            if (!IsRunning)
                throw new InvalidOperationException("Application is not running.");

            Window.Dispose();
            InvokeQueue.CompleteAdding();

            Logger.Info("Shutdown.");
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

            Logger.Info("Shutdown with exception.", e);
        }
        #endregion
        #region IsLocalUrl
        /// <summary>
        /// Determines whether the specified URL is local.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        internal bool IsLocalUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            return UrlUtility.IsLocalUrl(RequestHostname, NativeRequestPort, url);
        }
        #endregion
        #region IsNativeRequestUrl
        /// <summary>
        /// Determines whether the specified URL is native request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        internal bool IsNativeRequestUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            return UrlUtility.IsNativeRequestUrl(RequestHostname, NativeRequestPort, url);
        }
        #endregion
        #region GetNativeRequestPath
        /// <summary>
        /// Gets the native request path.
        /// </summary>
        /// <param name="absoluteNativeRequestUrl">The absolute native request URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">absoluteNativeRequestUrl</exception>
        /// <exception cref="System.ArgumentException">Invalid url.;absoluteContentUrl</exception>
        public string GetNativeRequestPath(string absoluteNativeRequestUrl)
        {
            if (string.IsNullOrWhiteSpace(absoluteNativeRequestUrl))
                throw new ArgumentNullException("absoluteNativeRequestUrl");

            var match = Regex.Match(absoluteNativeRequestUrl,
                string.Format("^(http://)?{0}:{1}/(.+)$",
                    Regex.Escape(RequestHostname),
                    NativeRequestPort),
                RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new ArgumentException("Invalid url.", "absoluteNativeRequestUrl");

            return match.Groups.OfType<Group>().Last().Value;
        }
        #endregion
        #region GetControllerNames
        /// <summary>
        /// Gets the controller names.
        /// </summary>
        /// <returns></returns>
        internal List<string> GetControllerNames()
        {
            return Application.Current.ControllerProvider.ControllerTypes
                .Select(c => c.Name).ToList();
        }
        #endregion
        #region InvokeOnCef
        /// <summary>
        /// Invokes the on cef asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        internal Task InvokeOnCefAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var taskCompletionSource = new TaskCompletionSource<object>();

            if (IsMultiThreadedMessageLoop)
                return InvokeOnMainAsync(action);

            CefUtility.ExecuteTask(CefThreadId.UI, () =>
            {
                try
                {
                    action();

                    taskCompletionSource.SetResult(null);
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            });

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Invokes the on cef.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        internal void InvokeOnCef(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (!IsCefThread)
                InvokeOnCefAsync(action).Wait();
            else
                action();
        }
        #endregion
        #region EnsureCefThread
        /// <summary>
        /// Ensures that it's called from the Cef thread.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Must be called from the Cef thread.</exception>
        internal void EnsureCefThread()
        {
            if (!IsCefThread)
                throw new InvalidOperationException("Must be called from the Cef thread.");
        }
        #endregion

        #endregion
        #region Protected

        #region RunInternal
        /// <summary>
        /// Run internal.
        /// </summary>
        protected sealed override void RunInternal()
        {
            try
            {
                InitializeCef();

                OnInitialize();
                RunMessageLoop();

                ShutdownInternal();
            }
            finally
            {
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
        private void InitializeCef()
        {
            if (IsMultiThreadedMessageLoop)
            {
                InitializeCefOnCurrentThread();
            }
            else
            {
                var cefInitialized = new AutoResetEvent(false);

                CefThread = new Thread(() =>
                {
                    Logger.Info("CEF main application thread created.");

                    InitializeCefOnCurrentThread();

                    cefInitialized.Set();

                    SingleThreadedMessageLoopStoped = new AutoResetEvent(false);

                    Logger.Info("CEF message loop started.");

                    CefRuntime.RunMessageLoop();
                    CefRuntime.Shutdown();

                    SingleThreadedMessageLoopStoped.Set();

                    Logger.Info("CEF main application thread destroyed.");
                });
                CefThread.SetApartmentState(ApartmentState.STA);
                CefThread.Start();

                cefInitialized.WaitOne();
            }
        }

        /// <summary>
        /// Initializes the cef on current thread.
        /// </summary>
        private void InitializeCefOnCurrentThread()
        {
            Logger.Info("Initializing CEF.");

            CefRuntime.Load();

            var cefSettings = new CefSettings
            {
                MultiThreadedMessageLoop = IsMultiThreadedMessageLoop,
                SingleProcess = false,
                LogSeverity = CefLogSeverity.Warning,
                LogFile = LogsDirectoryPath + "/cef.log",
                CachePath = CacheDirectoryPath,
                ResourcesDirPath = PathUtility.WorkingDirectory,
                LocalesDirPath = PathUtility.WorkingDirectory + "/locales",
                RemoteDebuggingPort = RemoteDebuggingPort,
                NoSandbox = true,
                CommandLineArgsDisabled = !CommandLineArgsEnabled
            };

            // arguments
            var arguments = new List<string>();

            if (CommandLineArgsEnabled)
                arguments.AddRange(EnvironmentUtility.GetCommandLineArgs());

            if (!D3D11Enabled && !arguments.Contains(Argument.DisableD3D11.Value))
                arguments.Add(Argument.DisableD3D11.Value);

            // initialize
            var mainArgs = new CefMainArgs(arguments.ToArray());
            var app = new App();

            CefRuntime.Initialize(mainArgs, cefSettings, app, IntPtr.Zero);

            Logger.Info("CEF initialized.");
        }
        #endregion
        #region ShutdownInternal
        /// <summary>
        /// Shutdowns cef.
        /// </summary>
        private void ShutdownInternal()
        {
            Logger.Info("CEF shutdown.");

            InvokeOnCef(() =>
            {
                if (IsMultiThreadedMessageLoop)
                    CefRuntime.Shutdown();
                else
                    CefRuntime.QuitMessageLoop();
            });

            if (!IsMultiThreadedMessageLoop)
                SingleThreadedMessageLoopStoped.WaitOne();
        }
        #endregion
        #region RunMessageLoop
        /// <summary>
        /// Runs the message loop.
        /// </summary>
        private void RunMessageLoop()
        {
            Logger.Info("Message loop started.");

            foreach (var action in InvokeQueue.GetConsumingEnumerable())
                action();

            Logger.Info("Message loop stoped.");

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
        #region GetDefaultMimeTypes
        /// <summary>
        /// Gets the default MIME types.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetDefaultMimeTypes()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(ResourceUtility.GetResourceAsString("MimeTypes.json"));
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
        protected override void Dispose(bool disposing)
        {
            EnsureMainThread();

            if (!_disposed)
            {
                if (disposing)
                {
                    if (InvokeQueue != null)
                        InvokeQueue.Dispose();
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
