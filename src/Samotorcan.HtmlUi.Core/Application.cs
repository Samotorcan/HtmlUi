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
                EnsureMainThread();

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
        #region Private

        #region ExitMessageLoop
        /// <summary>
        /// Gets or sets a value indicating whether to exit message loop.
        /// </summary>
        /// <value>
        ///   <c>true</c> to exit message loop; otherwise, <c>false</c>.
        /// </value>
        private bool ExitMessageLoop { get; set; }
        #endregion
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
        private Queue<Action> InvokeQueue { get; set; }
        #endregion
        #region InvokeQueueLock
        /// <summary>
        /// Gets or sets the invoke queue lock.
        /// </summary>
        /// <value>
        /// The invoke queue lock.
        /// </value>
        private object InvokeQueueLock { get; set; }
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

            ThreadId = Thread.CurrentThread.ManagedThreadId;
            Current = this;

            InvokeQueue = new Queue<Action>();
            InvokeQueueLock = new object();
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

            try
            {
                InitializeCef();

                RegisterSchemes();
                RegisterMessageRouter();

                Initialize();
                RunMessageLoop();

                ShutdownInternal();
            }
            catch (Exception e)
            {
                GeneralLog.Warn("Exception while trying to run the application.", e);
                throw;
            }
            finally
            {
                IsRunning = false;
            }
        }
        #endregion
        #region InvokeOnMain
        /// <summary>
        /// Invokes the specified action on the main thread.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public void InvokeOnMain(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (IsMainThread)
                action();

            lock (InvokeQueueLock)
            {
                InvokeQueue.Enqueue(action);
            }
        }
        #endregion

        #endregion
        #region Protected

        #region Initialize
        /// <summary>
        /// Initialize.
        /// </summary>
        protected virtual void Initialize() { }
        #endregion
        #region CreateWindow
        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <returns></returns>
        protected abstract Window CreateWindow();
        #endregion
        #region InitializeWindow
        /// <summary>
        /// Initializes the window.
        /// </summary>
        protected void InitializeWindow()
        {
            if (Window != null)
                throw new InvalidOperationException("Window already initialized.");

            Window = CreateWindow();
        }
        #endregion
        #region OnShutdown
        /// <summary>
        /// Called when shutdown is called.
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

            ExitMessageLoop = true;
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
        }
        #endregion
        #region RunMessageLoop
        /// <summary>
        /// Runs the message loop.
        /// </summary>
        private void RunMessageLoop()
        {
            while (!ExitMessageLoop)
            {
                // process invoke actions
                if (InvokeQueue.Count > 0)
                {
                    List<Action> actions = null;

                    // copy actions to list
                    lock (InvokeQueueLock)
                    {
                        actions = InvokeQueue.ToList();
                        InvokeQueue.Clear();
                    }

                    // run actions
                    foreach (var action in actions)
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception e)
                        {
                            GeneralLog.Error("Application invoke exception.", e);
                        }
                    }

                    Thread.Sleep(0);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }

            OnShutdown();
            ExitMessageLoop = false;
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
            EnsureMainThread();

            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
