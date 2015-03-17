using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Scheme;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;
using System.Diagnostics;
using System.Threading;
using Samotorcan.HtmlUi.Core.Utilities;
using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Core.Exceptions;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Samotorcan.HtmlUi.Core.Browser;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Base main application.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class BaseMainApplication : BaseApplication
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
        public static new BaseMainApplication Current
        {
            get
            {
                return (BaseMainApplication)BaseApplication.Current;
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
                if (Window.IsBrowserCreated)
                    throw new InvalidOperationException("EnableD3D11 can only be changed before the window is created.");

                _enableD3D11 = value;
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

        #region BrowserMessageRouter
        /// <summary>
        /// Gets or sets the browser message router.
        /// </summary>
        /// <value>
        /// The browser message router.
        /// </value>
        internal CefMessageRouterBrowserSide BrowserMessageRouter { get; set; }
        #endregion
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

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMainApplication"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">You can only have one instance of Application at any given time.</exception>
        protected BaseMainApplication()
            : base()
        {
            if (HtmlUiRuntime.ApplicationType != ApplicationType.MainApplication)
                throw new InvalidOperationException("Application must be run on main application process.");

            SynchronizationContext.SetSynchronizationContext(new HtmlUiSynchronizationContext());

            InitializeInvokeQueue();

            ContentProvider = new FileAssemblyContentProvider();
            ControllerProvider = new AssemblyControllerProvider();
            RequestHostname = "localhost";
            NativeRequestPort = 16556;

            MimeTypes = GetDefaultMimeTypes();
            SyncMaxDepth = 10;
        }

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

            GeneralLog.Debug("Invoke on main called.");

            var taskCompletionSource = new TaskCompletionSource<object>();

            InvokeQueue.Add(() =>
            {
                try
                {
                    action();
                    Window.SyncControllerChangesToClient();

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
        #region GetAbsoluteContentUrl
        /// <summary>
        /// Gets the absolute content URL.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        /// <returns></returns>
        public string GetAbsoluteContentUrl(string contentPath)
        {
            if (string.IsNullOrWhiteSpace(contentPath))
                throw new ArgumentNullException("contentPath");

            return string.Format("http://{0}/{1}",
                RequestHostname,
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
            if (string.IsNullOrWhiteSpace(absoluteContentUrl))
                throw new ArgumentNullException("absoluteContentUrl");

            var match = Regex.Match(absoluteContentUrl,
                string.Format("^(http://)?{0}(:80)?/(.+)$",
                    Regex.Escape(RequestHostname)),
                RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new ArgumentException("Invalid url.", "absoluteContentUrl");

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

            GeneralLog.Info("Shutdown.");
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

            GeneralLog.Info("Shutdown with exception.", e);
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

            return Regex.IsMatch(url, string.Format("^(http://)?{0}((:80)?|:{1})(/.*)?$",
                    Regex.Escape(RequestHostname),
                    NativeRequestPort),
                RegexOptions.IgnoreCase);
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

            return Regex.IsMatch(url, string.Format("^(http://)?{0}:{1}(/.*)?$",
                    Regex.Escape(RequestHostname),
                    NativeRequestPort),
                RegexOptions.IgnoreCase);
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
            return BaseMainApplication.Current.ControllerProvider.ControllerTypes
                .Select(c => c.Name).ToList();
        }
        #endregion

        #endregion
        #region Protected

        #region RunInternal
        /// <summary>
        /// Run internal.
        /// </summary>
        protected override void RunInternal()
        {
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
            if (!EnableD3D11 && !arguments.Contains(Argument.DisableD3D11.Value))
                arguments.Add(Argument.DisableD3D11.Value);

            // initialize
            var mainArgs = new CefMainArgs(arguments.ToArray());
            var app = new App();

            CefRuntime.Initialize(mainArgs, cefSettings, app, IntPtr.Zero);
        }
        #endregion
        #region RegisterSchemes
        /// <summary>
        /// Registers the schemes.
        /// </summary>
        private void RegisterSchemes()
        {
            // TODO: Temp.
            CefRuntime.RegisterSchemeHandlerFactory("http", "test.test.test", new DefaultAppSchemeHandlerFactory());

            GeneralLog.Info("Schemes registered.");
        }
        #endregion
        #region RegisterMessageRouter
        /// <summary>
        /// Registers the message router.
        /// </summary>
        private void RegisterMessageRouter()
        {
            BrowserMessageRouter = new CefMessageRouterBrowserSide(new CefMessageRouterConfig());

            GeneralLog.Info("Message route registered.");
        }
        #endregion
        #region ShutdownInternal
        /// <summary>
        /// Shutdowns cef.
        /// </summary>
        private void ShutdownInternal()
        {
            CefRuntime.Shutdown();

            GeneralLog.Info("CEF shutdown.");
        }
        #endregion
        #region RunMessageLoop
        /// <summary>
        /// Runs the message loop.
        /// </summary>
        private void RunMessageLoop()
        {
            GeneralLog.Info("Message loop started.");

            foreach (var action in InvokeQueue.GetConsumingEnumerable())
                action();

            GeneralLog.Info("Message loop stoped.");

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
