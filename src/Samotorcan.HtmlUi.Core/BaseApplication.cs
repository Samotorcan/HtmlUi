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
                if (Window.IsBrowserCreated)
                    throw new InvalidOperationException("EnableD3D11 can only be changed before the window is created.");

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
        #region MimeTypes
        /// <summary>
        /// Gets the MIME types.
        /// </summary>
        /// <value>
        /// The MIME types.
        /// </value>
        public Dictionary<string, string> MimeTypes { get; private set; }
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
        #region LogsDirectoryPath
        /// <summary>
        /// Gets or sets the logs directory path.
        /// </summary>
        /// <value>
        /// The logs directory path.
        /// </value>
        internal string LogsDirectoryPath { get; set; }
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
            if (Current != null)
                throw new InvalidOperationException("You can only have one instance of Application at any given time.");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            LogsDirectoryPath = PathUtility.NormalizedWorkingDirectory + "/" + LogsDirectory;
            EnsureLogsDirectory();
            log4net.GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;

            ThreadId = Thread.CurrentThread.ManagedThreadId;
            Current = this;

            SynchronizationContext.SetSynchronizationContext(new HtmlUiSynchronizationContext());

            InitializeInvokeQueue();

            ContentProvider = new FileAssemblyContentProvider();
            ControllerProvider = new AssemblyControllerProvider();
            RequestHostname = "localhost";
            NativeRequestPort = 16556;

            MimeTypes = GetDefaultMimeTypes();
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

            if (IsRunning)
                throw new InvalidOperationException("Application is already running.");

            IsRunning = true;
            GeneralLog.Info("Application Run started.");

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

            GeneralLog.Info("Application Run ended.");
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
            if (action == null)
                throw new ArgumentNullException("action");

            if (InvokeQueue.IsAddingCompleted)
                throw new InvalidOperationException("Application is shutting down.");

            GeneralLog.Info("Invoke on main called.");

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
            if (action == null)
                throw new ArgumentNullException("action");

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

        #region EnsureMainThread
        /// <summary>
        /// Ensures that it's called from the main thread.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Must be called from the main thread.</exception>
        internal void EnsureMainThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != ThreadId)
                throw new InvalidOperationException("Must be called from the main thread.");
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
        #region Digest
        /// <summary>
        /// Digest call. Updates the controllers.
        /// </summary>
        /// <param name="controllerChanges">The controller changes.</param>
        internal void Digest(IEnumerable<ControllerChange> controllerChanges)
        {
            EnsureMainThread();

            GeneralLog.Info(string.Format("Digest call: {0}", JsonConvert.SerializeObject(controllerChanges)));

            foreach (var controllerChange in controllerChanges)
            {
                // TODO: implement
            }
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
            var processArguments = Environment.GetCommandLineArgs();
            var type = processArguments.Where(a => a.StartsWith("--type=")).Select(a => a.Substring(7)).FirstOrDefault();

            var arguments = !string.IsNullOrEmpty(type) ? processArguments.ToList() : new List<string>();
            if (!EnableD3D11 && !arguments.Contains(CefArgument.DisableD3D11.Value))
                arguments.Add(CefArgument.DisableD3D11.Value);

            // initialize
            var mainArgs = new CefMainArgs(arguments.ToArray());
            var app = new DefaultCefApp();

            // sub process
            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
            if (exitCode != -1)
            {
                Dispose();
                Environment.Exit(0);
            }

            CefRuntime.Initialize(mainArgs, cefSettings, app, IntPtr.Zero);
            GeneralLog.Info("CEF initialized.");
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

            GeneralLog.Info("Message lopp stoped.");

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
        #region GetDefaultMimeTypes
        /// <summary>
        /// Gets the default MIME types.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetDefaultMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".323", "text/h323"},
                {".3g2", "video/3gpp2"},
                {".3gp", "video/3gpp"},
                {".3gp2", "video/3gpp2"},
                {".3gpp", "video/3gpp"},
                {".7z", "application/x-7z-compressed"},
                {".aa", "audio/audible"},
                {".AAC", "audio/aac"},
                {".aaf", "application/octet-stream"},
                {".aax", "audio/vnd.audible.aax"},
                {".ac3", "audio/ac3"},
                {".aca", "application/octet-stream"},
                {".accda", "application/msaccess.addin"},
                {".accdb", "application/msaccess"},
                {".accdc", "application/msaccess.cab"},
                {".accde", "application/msaccess"},
                {".accdr", "application/msaccess.runtime"},
                {".accdt", "application/msaccess"},
                {".accdw", "application/msaccess.webapplication"},
                {".accft", "application/msaccess.ftemplate"},
                {".acx", "application/internet-property-stream"},
                {".AddIn", "text/xml"},
                {".ade", "application/msaccess"},
                {".adobebridge", "application/x-bridge-url"},
                {".adp", "application/msaccess"},
                {".ADT", "audio/vnd.dlna.adts"},
                {".ADTS", "audio/aac"},
                {".afm", "application/octet-stream"},
                {".ai", "application/postscript"},
                {".aif", "audio/x-aiff"},
                {".aifc", "audio/aiff"},
                {".aiff", "audio/aiff"},
                {".air", "application/vnd.adobe.air-application-installer-package+zip"},
                {".amc", "application/x-mpeg"},
                {".application", "application/x-ms-application"},
                {".art", "image/x-jg"},
                {".asa", "application/xml"},
                {".asax", "application/xml"},
                {".ascx", "application/xml"},
                {".asd", "application/octet-stream"},
                {".asf", "video/x-ms-asf"},
                {".ashx", "application/xml"},
                {".asi", "application/octet-stream"},
                {".asm", "text/plain"},
                {".asmx", "application/xml"},
                {".aspx", "application/xml"},
                {".asr", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".atom", "application/atom+xml"},
                {".au", "audio/basic"},
                {".avi", "video/x-msvideo"},
                {".axs", "application/olescript"},
                {".bas", "text/plain"},
                {".bcpio", "application/x-bcpio"},
                {".bin", "application/octet-stream"},
                {".bmp", "image/bmp"},
                {".c", "text/plain"},
                {".cab", "application/octet-stream"},
                {".caf", "audio/x-caf"},
                {".calx", "application/vnd.ms-office.calx"},
                {".cat", "application/vnd.ms-pki.seccat"},
                {".cc", "text/plain"},
                {".cd", "text/plain"},
                {".cdda", "audio/aiff"},
                {".cdf", "application/x-cdf"},
                {".cer", "application/x-x509-ca-cert"},
                {".chm", "application/octet-stream"},
                {".class", "application/x-java-applet"},
                {".clp", "application/x-msclip"},
                {".cmx", "image/x-cmx"},
                {".cnf", "text/plain"},
                {".cod", "image/cis-cod"},
                {".config", "application/xml"},
                {".contact", "text/x-ms-contact"},
                {".coverage", "application/xml"},
                {".cpio", "application/x-cpio"},
                {".cpp", "text/plain"},
                {".crd", "application/x-mscardfile"},
                {".crl", "application/pkix-crl"},
                {".crt", "application/x-x509-ca-cert"},
                {".cs", "text/plain"},
                {".csdproj", "text/plain"},
                {".csh", "application/x-csh"},
                {".csproj", "text/plain"},
                {".css", "text/css"},
                {".csv", "text/csv"},
                {".cur", "application/octet-stream"},
                {".cxx", "text/plain"},
                {".dat", "application/octet-stream"},
                {".datasource", "application/xml"},
                {".dbproj", "text/plain"},
                {".dcr", "application/x-director"},
                {".def", "text/plain"},
                {".deploy", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dgml", "application/xml"},
                {".dib", "image/bmp"},
                {".dif", "video/x-dv"},
                {".dir", "application/x-director"},
                {".disco", "text/xml"},
                {".dll", "application/x-msdownload"},
                {".dll.config", "text/xml"},
                {".dlm", "text/dlm"},
                {".doc", "application/msword"},
                {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".dot", "application/msword"},
                {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
                {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
                {".dsp", "application/octet-stream"},
                {".dsw", "text/plain"},
                {".dtd", "text/xml"},
                {".dtsConfig", "text/xml"},
                {".dv", "video/x-dv"},
                {".dvi", "application/x-dvi"},
                {".dwf", "drawing/x-dwf"},
                {".dwp", "application/octet-stream"},
                {".dxr", "application/x-director"},
                {".eml", "message/rfc822"},
                {".emz", "application/octet-stream"},
                {".eot", "application/octet-stream"},
                {".eps", "application/postscript"},
                {".etl", "application/etl"},
                {".etx", "text/x-setext"},
                {".evy", "application/envoy"},
                {".exe", "application/octet-stream"},
                {".exe.config", "text/xml"},
                {".fdf", "application/vnd.fdf"},
                {".fif", "application/fractals"},
                {".filters", "Application/xml"},
                {".fla", "application/octet-stream"},
                {".flr", "x-world/x-vrml"},
                {".flv", "video/x-flv"},
                {".fsscript", "application/fsharp-script"},
                {".fsx", "application/fsharp-script"},
                {".generictest", "application/xml"},
                {".gif", "image/gif"},
                {".group", "text/x-ms-group"},
                {".gsm", "audio/x-gsm"},
                {".gtar", "application/x-gtar"},
                {".gz", "application/x-gzip"},
                {".h", "text/plain"},
                {".hdf", "application/x-hdf"},
                {".hdml", "text/x-hdml"},
                {".hhc", "application/x-oleobject"},
                {".hhk", "application/octet-stream"},
                {".hhp", "application/octet-stream"},
                {".hlp", "application/winhlp"},
                {".hpp", "text/plain"},
                {".hqx", "application/mac-binhex40"},
                {".hta", "application/hta"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".htt", "text/webviewhtml"},
                {".hxa", "application/xml"},
                {".hxc", "application/xml"},
                {".hxd", "application/octet-stream"},
                {".hxe", "application/xml"},
                {".hxf", "application/xml"},
                {".hxh", "application/octet-stream"},
                {".hxi", "application/octet-stream"},
                {".hxk", "application/xml"},
                {".hxq", "application/octet-stream"},
                {".hxr", "application/octet-stream"},
                {".hxs", "application/octet-stream"},
                {".hxt", "text/html"},
                {".hxv", "application/xml"},
                {".hxw", "application/octet-stream"},
                {".hxx", "text/plain"},
                {".i", "text/plain"},
                {".ico", "image/x-icon"},
                {".ics", "application/octet-stream"},
                {".idl", "text/plain"},
                {".ief", "image/ief"},
                {".iii", "application/x-iphone"},
                {".inc", "text/plain"},
                {".inf", "application/octet-stream"},
                {".inl", "text/plain"},
                {".ins", "application/x-internet-signup"},
                {".ipa", "application/x-itunes-ipa"},
                {".ipg", "application/x-itunes-ipg"},
                {".ipproj", "text/plain"},
                {".ipsw", "application/x-itunes-ipsw"},
                {".iqy", "text/x-ms-iqy"},
                {".isp", "application/x-internet-signup"},
                {".ite", "application/x-itunes-ite"},
                {".itlp", "application/x-itunes-itlp"},
                {".itms", "application/x-itunes-itms"},
                {".itpc", "application/x-itunes-itpc"},
                {".IVF", "video/x-ivf"},
                {".jar", "application/java-archive"},
                {".java", "application/octet-stream"},
                {".jck", "application/liquidmotion"},
                {".jcz", "application/liquidmotion"},
                {".jfif", "image/pjpeg"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpb", "application/octet-stream"},
                {".jpe", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/x-javascript"},
                {".json", "application/json"},
                {".jsx", "text/jscript"},
                {".jsxbin", "text/plain"},
                {".latex", "application/x-latex"},
                {".library-ms", "application/windows-library+xml"},
                {".lit", "application/x-ms-reader"},
                {".loadtest", "application/xml"},
                {".lpk", "application/octet-stream"},
                {".lsf", "video/x-la-asf"},
                {".lst", "text/plain"},
                {".lsx", "video/x-la-asf"},
                {".lzh", "application/octet-stream"},
                {".m13", "application/x-msmediaview"},
                {".m14", "application/x-msmediaview"},
                {".m1v", "video/mpeg"},
                {".m2t", "video/vnd.dlna.mpeg-tts"},
                {".m2ts", "video/vnd.dlna.mpeg-tts"},
                {".m2v", "video/mpeg"},
                {".m3u", "audio/x-mpegurl"},
                {".m3u8", "audio/x-mpegurl"},
                {".m4a", "audio/m4a"},
                {".m4b", "audio/m4b"},
                {".m4p", "audio/m4p"},
                {".m4r", "audio/x-m4r"},
                {".m4v", "video/x-m4v"},
                {".mac", "image/x-macpaint"},
                {".mak", "text/plain"},
                {".man", "application/x-troff-man"},
                {".manifest", "application/x-ms-manifest"},
                {".map", "text/plain"},
                {".master", "application/xml"},
                {".mda", "application/msaccess"},
                {".mdb", "application/x-msaccess"},
                {".mde", "application/msaccess"},
                {".mdp", "application/octet-stream"},
                {".me", "application/x-troff-me"},
                {".mfp", "application/x-shockwave-flash"},
                {".mht", "message/rfc822"},
                {".mhtml", "message/rfc822"},
                {".mid", "audio/mid"},
                {".midi", "audio/mid"},
                {".mix", "application/octet-stream"},
                {".mk", "text/plain"},
                {".mmf", "application/x-smaf"},
                {".mno", "text/xml"},
                {".mny", "application/x-msmoney"},
                {".mod", "video/mpeg"},
                {".mov", "video/quicktime"},
                {".movie", "video/x-sgi-movie"},
                {".mp2", "video/mpeg"},
                {".mp2v", "video/mpeg"},
                {".mp3", "audio/mpeg"},
                {".mp4", "video/mp4"},
                {".mp4v", "video/mp4"},
                {".mpa", "video/mpeg"},
                {".mpe", "video/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpf", "application/vnd.ms-mediapackage"},
                {".mpg", "video/mpeg"},
                {".mpp", "application/vnd.ms-project"},
                {".mpv2", "video/mpeg"},
                {".mqv", "video/quicktime"},
                {".ms", "application/x-troff-ms"},
                {".msi", "application/octet-stream"},
                {".mso", "application/octet-stream"},
                {".mts", "video/vnd.dlna.mpeg-tts"},
                {".mtx", "application/xml"},
                {".mvb", "application/x-msmediaview"},
                {".mvc", "application/x-miva-compiled"},
                {".mxp", "application/x-mmxp"},
                {".nc", "application/x-netcdf"},
                {".nsc", "video/x-ms-asf"},
                {".nws", "message/rfc822"},
                {".ocx", "application/octet-stream"},
                {".oda", "application/oda"},
                {".odc", "text/x-ms-odc"},
                {".odh", "text/plain"},
                {".odl", "text/plain"},
                {".odp", "application/vnd.oasis.opendocument.presentation"},
                {".ods", "application/oleobject"},
                {".odt", "application/vnd.oasis.opendocument.text"},
                {".one", "application/onenote"},
                {".onea", "application/onenote"},
                {".onepkg", "application/onenote"},
                {".onetmp", "application/onenote"},
                {".onetoc", "application/onenote"},
                {".onetoc2", "application/onenote"},
                {".orderedtest", "application/xml"},
                {".osdx", "application/opensearchdescription+xml"},
                {".p10", "application/pkcs10"},
                {".p12", "application/x-pkcs12"},
                {".p7b", "application/x-pkcs7-certificates"},
                {".p7c", "application/pkcs7-mime"},
                {".p7m", "application/pkcs7-mime"},
                {".p7r", "application/x-pkcs7-certreqresp"},
                {".p7s", "application/pkcs7-signature"},
                {".pbm", "image/x-portable-bitmap"},
                {".pcast", "application/x-podcast"},
                {".pct", "image/pict"},
                {".pcx", "application/octet-stream"},
                {".pcz", "application/octet-stream"},
                {".pdf", "application/pdf"},
                {".pfb", "application/octet-stream"},
                {".pfm", "application/octet-stream"},
                {".pfx", "application/x-pkcs12"},
                {".pgm", "image/x-portable-graymap"},
                {".pic", "image/pict"},
                {".pict", "image/pict"},
                {".pkgdef", "text/plain"},
                {".pkgundef", "text/plain"},
                {".pko", "application/vnd.ms-pki.pko"},
                {".pls", "audio/scpls"},
                {".pma", "application/x-perfmon"},
                {".pmc", "application/x-perfmon"},
                {".pml", "application/x-perfmon"},
                {".pmr", "application/x-perfmon"},
                {".pmw", "application/x-perfmon"},
                {".png", "image/png"},
                {".pnm", "image/x-portable-anymap"},
                {".pnt", "image/x-macpaint"},
                {".pntg", "image/x-macpaint"},
                {".pnz", "image/png"},
                {".pot", "application/vnd.ms-powerpoint"},
                {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
                {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
                {".ppa", "application/vnd.ms-powerpoint"},
                {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
                {".ppm", "image/x-portable-pixmap"},
                {".pps", "application/vnd.ms-powerpoint"},
                {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
                {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".prf", "application/pics-rules"},
                {".prm", "application/octet-stream"},
                {".prx", "application/octet-stream"},
                {".ps", "application/postscript"},
                {".psc1", "application/PowerShell"},
                {".psd", "application/octet-stream"},
                {".psess", "application/xml"},
                {".psm", "application/octet-stream"},
                {".psp", "application/octet-stream"},
                {".pub", "application/x-mspublisher"},
                {".pwz", "application/vnd.ms-powerpoint"},
                {".qht", "text/x-html-insertion"},
                {".qhtm", "text/x-html-insertion"},
                {".qt", "video/quicktime"},
                {".qti", "image/x-quicktime"},
                {".qtif", "image/x-quicktime"},
                {".qtl", "application/x-quicktimeplayer"},
                {".qxd", "application/octet-stream"},
                {".ra", "audio/x-pn-realaudio"},
                {".ram", "audio/x-pn-realaudio"},
                {".rar", "application/octet-stream"},
                {".ras", "image/x-cmu-raster"},
                {".rat", "application/rat-file"},
                {".rc", "text/plain"},
                {".rc2", "text/plain"},
                {".rct", "text/plain"},
                {".rdlc", "application/xml"},
                {".resx", "application/xml"},
                {".rf", "image/vnd.rn-realflash"},
                {".rgb", "image/x-rgb"},
                {".rgs", "text/plain"},
                {".rm", "application/vnd.rn-realmedia"},
                {".rmi", "audio/mid"},
                {".rmp", "application/vnd.rn-rn_music_package"},
                {".roff", "application/x-troff"},
                {".rpm", "audio/x-pn-realaudio-plugin"},
                {".rqy", "text/x-ms-rqy"},
                {".rtf", "application/rtf"},
                {".rtx", "text/richtext"},
                {".ruleset", "application/xml"},
                {".s", "text/plain"},
                {".safariextz", "application/x-safari-safariextz"},
                {".scd", "application/x-msschedule"},
                {".sct", "text/scriptlet"},
                {".sd2", "audio/x-sd2"},
                {".sdp", "application/sdp"},
                {".sea", "application/octet-stream"},
                {".searchConnector-ms", "application/windows-search-connector+xml"},
                {".setpay", "application/set-payment-initiation"},
                {".setreg", "application/set-registration-initiation"},
                {".settings", "application/xml"},
                {".sgimb", "application/x-sgimb"},
                {".sgml", "text/sgml"},
                {".sh", "application/x-sh"},
                {".shar", "application/x-shar"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".sitemap", "application/xml"},
                {".skin", "application/xml"},
                {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
                {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
                {".slk", "application/vnd.ms-excel"},
                {".sln", "text/plain"},
                {".slupkg-ms", "application/x-ms-license"},
                {".smd", "audio/x-smd"},
                {".smi", "application/octet-stream"},
                {".smx", "audio/x-smd"},
                {".smz", "audio/x-smd"},
                {".snd", "audio/basic"},
                {".snippet", "application/xml"},
                {".snp", "application/octet-stream"},
                {".sol", "text/plain"},
                {".sor", "text/plain"},
                {".spc", "application/x-pkcs7-certificates"},
                {".spl", "application/futuresplash"},
                {".src", "application/x-wais-source"},
                {".srf", "text/plain"},
                {".SSISDeploymentManifest", "text/xml"},
                {".ssm", "application/streamingmedia"},
                {".sst", "application/vnd.ms-pki.certstore"},
                {".stl", "application/vnd.ms-pki.stl"},
                {".sv4cpio", "application/x-sv4cpio"},
                {".sv4crc", "application/x-sv4crc"},
                {".svc", "application/xml"},
                {".swf", "application/x-shockwave-flash"},
                {".t", "application/x-troff"},
                {".tar", "application/x-tar"},
                {".tcl", "application/x-tcl"},
                {".testrunconfig", "application/xml"},
                {".testsettings", "application/xml"},
                {".tex", "application/x-tex"},
                {".texi", "application/x-texinfo"},
                {".texinfo", "application/x-texinfo"},
                {".tgz", "application/x-compressed"},
                {".thmx", "application/vnd.ms-officetheme"},
                {".thn", "application/octet-stream"},
                {".tif", "image/tiff"},
                {".tiff", "image/tiff"},
                {".tlh", "text/plain"},
                {".tli", "text/plain"},
                {".toc", "application/octet-stream"},
                {".tr", "application/x-troff"},
                {".trm", "application/x-msterminal"},
                {".trx", "application/xml"},
                {".ts", "video/vnd.dlna.mpeg-tts"},
                {".tsv", "text/tab-separated-values"},
                {".ttf", "application/octet-stream"},
                {".tts", "video/vnd.dlna.mpeg-tts"},
                {".txt", "text/plain"},
                {".u32", "application/octet-stream"},
                {".uls", "text/iuls"},
                {".user", "text/plain"},
                {".ustar", "application/x-ustar"},
                {".vb", "text/plain"},
                {".vbdproj", "text/plain"},
                {".vbk", "video/mpeg"},
                {".vbproj", "text/plain"},
                {".vbs", "text/vbscript"},
                {".vcf", "text/x-vcard"},
                {".vcproj", "Application/xml"},
                {".vcs", "text/plain"},
                {".vcxproj", "Application/xml"},
                {".vddproj", "text/plain"},
                {".vdp", "text/plain"},
                {".vdproj", "text/plain"},
                {".vdx", "application/vnd.ms-visio.viewer"},
                {".vml", "text/xml"},
                {".vscontent", "application/xml"},
                {".vsct", "text/xml"},
                {".vsd", "application/vnd.visio"},
                {".vsi", "application/ms-vsi"},
                {".vsix", "application/vsix"},
                {".vsixlangpack", "text/xml"},
                {".vsixmanifest", "text/xml"},
                {".vsmdi", "application/xml"},
                {".vspscc", "text/plain"},
                {".vss", "application/vnd.visio"},
                {".vsscc", "text/plain"},
                {".vssettings", "text/xml"},
                {".vssscc", "text/plain"},
                {".vst", "application/vnd.visio"},
                {".vstemplate", "text/xml"},
                {".vsto", "application/x-ms-vsto"},
                {".vsw", "application/vnd.visio"},
                {".vsx", "application/vnd.visio"},
                {".vtx", "application/vnd.visio"},
                {".wav", "audio/wav"},
                {".wave", "audio/wav"},
                {".wax", "audio/x-ms-wax"},
                {".wbk", "application/msword"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wcm", "application/vnd.ms-works"},
                {".wdb", "application/vnd.ms-works"},
                {".wdp", "image/vnd.ms-photo"},
                {".webarchive", "application/x-safari-webarchive"},
                {".webtest", "application/xml"},
                {".wiq", "application/xml"},
                {".wiz", "application/msword"},
                {".wks", "application/vnd.ms-works"},
                {".WLMP", "application/wlmoviemaker"},
                {".wlpginstall", "application/x-wlpg-detect"},
                {".wlpginstall3", "application/x-wlpg3-detect"},
                {".wm", "video/x-ms-wm"},
                {".wma", "audio/x-ms-wma"},
                {".wmd", "application/x-ms-wmd"},
                {".wmf", "application/x-msmetafile"},
                {".wml", "text/vnd.wap.wml"},
                {".wmlc", "application/vnd.wap.wmlc"},
                {".wmls", "text/vnd.wap.wmlscript"},
                {".wmlsc", "application/vnd.wap.wmlscriptc"},
                {".wmp", "video/x-ms-wmp"},
                {".wmv", "video/x-ms-wmv"},
                {".wmx", "video/x-ms-wmx"},
                {".wmz", "application/x-ms-wmz"},
                {".wpl", "application/vnd.ms-wpl"},
                {".wps", "application/vnd.ms-works"},
                {".wri", "application/x-mswrite"},
                {".wrl", "x-world/x-vrml"},
                {".wrz", "x-world/x-vrml"},
                {".wsc", "text/scriptlet"},
                {".wsdl", "text/xml"},
                {".wvx", "video/x-ms-wvx"},
                {".x", "application/directx"},
                {".xaf", "x-world/x-vrml"},
                {".xaml", "application/xaml+xml"},
                {".xap", "application/x-silverlight-app"},
                {".xbap", "application/x-ms-xbap"},
                {".xbm", "image/x-xbitmap"},
                {".xdr", "text/plain"},
                {".xht", "application/xhtml+xml"},
                {".xhtml", "application/xhtml+xml"},
                {".xla", "application/vnd.ms-excel"},
                {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
                {".xlc", "application/vnd.ms-excel"},
                {".xld", "application/vnd.ms-excel"},
                {".xlk", "application/vnd.ms-excel"},
                {".xll", "application/vnd.ms-excel"},
                {".xlm", "application/vnd.ms-excel"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
                {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".xlt", "application/vnd.ms-excel"},
                {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
                {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
                {".xlw", "application/vnd.ms-excel"},
                {".xml", "text/xml"},
                {".xmta", "application/xml"},
                {".xof", "x-world/x-vrml"},
                {".XOML", "text/plain"},
                {".xpm", "image/x-xpixmap"},
                {".xps", "application/vnd.ms-xpsdocument"},
                {".xrm-ms", "text/xml"},
                {".xsc", "application/xml"},
                {".xsd", "text/xml"},
                {".xsf", "text/xml"},
                {".xsl", "text/xml"},
                {".xslt", "text/xml"},
                {".xsn", "application/octet-stream"},
                {".xss", "application/xml"},
                {".xtp", "application/octet-stream"},
                {".xwd", "image/x-xwindowdump"},
                {".z", "application/x-compress"},
                {".zip", "application/x-zip-compressed"}
            };
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
