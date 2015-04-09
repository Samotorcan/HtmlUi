using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            CacheDirectoryPath = PathUtility.NormalizedWorkingDirectory + "/" + CacheDirectory;
            EnsureCacheDirectory();

            ThreadId = Thread.CurrentThread.ManagedThreadId;
            Current = this;
        }

        #endregion
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Application is already running.</exception>
        public void Run()
        {
            EnsureMainThread();

            if (IsRunning)
                throw new InvalidOperationException("Application is already running.");

            IsRunning = true;
            GeneralLog.Info("Application Run started.");

            try
            {
                RunInternal();
            }
            finally
            {
                IsRunning = false;
            }

            GeneralLog.Info("Application Run ended.");
        }
        #endregion

        #endregion
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

        #endregion
        #region Protected

        #region RunInternal
        /// <summary>
        /// Run internal.
        /// </summary>
        protected abstract void RunInternal();
        #endregion

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
