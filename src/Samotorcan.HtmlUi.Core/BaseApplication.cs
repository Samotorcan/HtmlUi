using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Samotorcan.HtmlUi.Core.Attributes;
using Samotorcan.HtmlUi.Core.Metadata;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Base application.
    /// </summary>
    public abstract class BaseApplication : IDisposable
    {
        #region Constants

        #region LogsDirectory
        /// <summary>
        /// The logs directory.
        /// </summary>
        internal const string LogsDirectory = "Logs";
        #endregion
        #region CacheDirectory
        /// <summary>
        /// The cache directory.
        /// </summary>
        internal const string CacheDirectory = "Cache";
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
        #region IsRunning
        /// <summary>
        /// Gets or sets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; private set; }
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
        [SyncProperty]
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

                if (_requestHostname != value)
                {
                    _requestHostname = value;

                    SyncProperty("RequestHostname", _requestHostname);
                }
            }
        }
        #endregion
        #region IncludeHtmUiScriptMapping
        private bool _includeHtmUiScriptMapping;
        /// <summary>
        /// Gets or sets a value indicating whether to include HTM UI script mapping.
        /// </summary>
        /// <value>
        /// <c>true</c> to include HTM UI script mapping; otherwise, <c>false</c>.
        /// </value>
        [SyncProperty]
        public bool IncludeHtmUiScriptMapping
        {
            get
            {
                return _includeHtmUiScriptMapping;
            }
            protected set
            {
                if (_includeHtmUiScriptMapping != value)
                {
                    _includeHtmUiScriptMapping = value;

                    SyncProperty("IncludeHtmUiScriptMapping", _includeHtmUiScriptMapping);
                }
            }
        }
        #endregion
        [SyncProperty]
        public LogSeverity LogSeverity
        {
            get
            {
                return Logger.LogSeverity;
            }
            protected set
            {
                if (Logger.LogSeverity != value)
                {
                    Logger.LogSeverity = value;

                    SyncProperty("LogSeverity", Logger.LogSeverity);
                }
            }
        }

        #endregion
        #region Internal

        #region LogsDirectoryPath
        /// <summary>
        /// Gets or sets the logs directory path.
        /// </summary>
        /// <value>
        /// The logs directory path.
        /// </value>
        internal string LogsDirectoryPath { get; set; }
        #endregion
        #region CacheDirectoryPath
        /// <summary>
        /// Gets or sets the cache directory path.
        /// </summary>
        /// <value>
        /// The cache directory path.
        /// </value>
        internal string CacheDirectoryPath { get; set; }
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
        [SyncProperty]
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

                if (_nativeRequestPort != value)
                {
                    _nativeRequestPort = value;

                    SyncProperty("NativeRequestPort", _nativeRequestPort);
                }
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
        #region OneApplicationCheckLock
        /// <summary>
        /// The one application check lock.
        /// </summary>
        private static object OneApplicationCheckLock = new object();
        #endregion
        private bool SyncPropertyDisabled { get; set; }
        private Dictionary<string, SyncProperty> SyncProperties { get; set; }

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApplication"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.InvalidOperationException">You can only have one instance of Application at any given time.</exception>
        protected BaseApplication(BaseApplicationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            lock (OneApplicationCheckLock)
            {
                if (Current != null)
                    throw new InvalidOperationException("You can only have one instance of Application at any given time.");

                Current = this;
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            LogsDirectoryPath = PathUtility.WorkingDirectory + "/" + LogsDirectory;
            EnsureLogsDirectory();

            InitializeLog4Net();

            CacheDirectoryPath = PathUtility.WorkingDirectory + "/" + CacheDirectory;
            EnsureCacheDirectory();

            ThreadId = Thread.CurrentThread.ManagedThreadId;

            SyncProperties = SyncPropertyAttribute.GetProperties<BaseApplication>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApplication"/> class with default settings.
        /// </summary>
        protected BaseApplication()
            : this(new BaseApplicationSettings()) { }

        #endregion
        #region Methods
        #region Internal

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
        #region RunApplication
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Application is already running.</exception>
        internal void RunApplication()
        {
            EnsureMainThread();

            if (IsRunning)
                throw new InvalidOperationException("Application is already running.");

            IsRunning = true;
            Logger.Info("Application Run started.");

            try
            {
                RunInternal();
            }
            finally
            {
                IsRunning = false;
            }

            Logger.Info("Application Run ended.");
        }
        #endregion
        internal void SetSyncProperty(string name, object value)
        {
            EnsureMainThread();

            SyncPropertyDisabled = true;
            try
            {
                SyncProperty property;
                SyncProperties.TryGetValue(name, out property);

                if (property == null)
                    throw new ArgumentException("Sync property not found.", "name");

                try
                {
                    property.SetDelegate.DynamicInvoke(this, ConvertUtility.ChangeType(value, property.SyncPropertyType));
                }
                catch
                {
                    throw new ArgumentException("Invalid sync property value.", "value");
                }
            }
            finally
            {
                SyncPropertyDisabled = false;
            }
        }

        #endregion
        #region Protected

        #region RunInternal
        /// <summary>
        /// Run internal.
        /// </summary>
        protected abstract void RunInternal();
        #endregion
        protected abstract void SyncPropertyInternal(string name, object value);
        protected void SyncProperty(string name, object value)
        {
            EnsureMainThread();

            if (!SyncPropertyDisabled)
                SyncPropertyInternal(name, value);
        }

        #endregion
        #region Private

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
        #region EnsureCacheDirectory
        /// <summary>
        /// Ensures the cache directory.
        /// </summary>
        private void EnsureCacheDirectory()
        {
            if (!Directory.Exists(CacheDirectoryPath))
                Directory.CreateDirectory(CacheDirectoryPath);
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
            Logger.Error("Unhandled exception.", e.ExceptionObject as Exception);
        }
        #endregion
        #region InitializeLog4Net
        /// <summary>
        /// Initializes the log4net.
        /// </summary>
        private void InitializeLog4Net()
        {
            using (var stream = ResourceUtility.GetResourceAsStream("log4net.config"))
            {
                log4net.Config.XmlConfigurator.Configure(stream);
            }
            log4net.GlobalContext.Properties["process"] = HtmlUiRuntime.ApplicationType == ApplicationType.Application ? "MAIN" : "RENDER";
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
