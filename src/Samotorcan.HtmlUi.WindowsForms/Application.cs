using Samotorcan.HtmlUi.Core.Logs;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormsApplication = System.Windows.Forms.Application;

namespace Samotorcan.HtmlUi.WindowsForms
{
    /// <summary>
    /// Windows forms application.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class Application : Core.Application
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
                return (Application)Core.Application.Current;
            }
        }
        #endregion
        #region IsUiThread
        /// <summary>
        /// Gets a value indicating whether current thread is main thread.
        /// </summary>
        /// <value>
        /// <c>true</c> if current thread is main thread; otherwise, <c>false</c>.
        /// </value>
        public bool IsUiThread
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId == UiThread.ManagedThreadId;
            }
        }
        #endregion

        #endregion
        #region Internal

        #region Window
        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        public new Window Window
        {
            get
            {
                return (Window)base.Window;
            }
            protected set
            {
                base.Window = value;
            }
        }
        #endregion

        #endregion
        #region Protected

        #region Settings
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        protected ApplicationContext Settings { get; set; }
        #endregion

        #endregion
        #region Private

        #region UiThread
        /// <summary>
        /// Gets or sets the UI thread.
        /// </summary>
        /// <value>
        /// The UI thread.
        /// </value>
        private Thread UiThread { get; set; }
        #endregion
        #region UiSynchronizationContext
        /// <summary>
        /// Gets or sets the UI synchronization context.
        /// </summary>
        /// <value>
        /// The UI synchronization context.
        /// </value>
        private WindowsFormsSynchronizationContext UiSynchronizationContext { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        protected Application(ApplicationContext settings)
            : base(settings)
        {
            Settings = settings;

            var uiSynchronizationContextCreated = new AutoResetEvent(false);

            UiThread = new Thread(() =>
            {
                FormsApplication.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                FormsApplication.ThreadException += new ThreadExceptionEventHandler(Forms_UiUnhandledException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Forms_UnhandledException);

                FormsApplication.EnableVisualStyles();
                FormsApplication.SetCompatibleTextRenderingDefault(false);

                SynchronizationContext.SetSynchronizationContext(UiSynchronizationContext = new WindowsFormsSynchronizationContext());
                uiSynchronizationContextCreated.Set();

                FormsApplication.Run();
            });
            UiThread.SetApartmentState(ApartmentState.STA);
            UiThread.Start();

            uiSynchronizationContextCreated.WaitOne();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class with default settings.
        /// </summary>
        protected Application()
            : this(new ApplicationContext()) { }

        #endregion
        #region Methods
        #region Public

        #region InvokeOnUi
        /// <summary>
        /// Invokes the specified action on the UI thread asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public Task InvokeOnUiAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var taskCompletionSource = new TaskCompletionSource<object>();

            UiSynchronizationContext.Post((state) =>
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
            }, null);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Invokes the specified action on the UI thread synchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        public void InvokeOnUi(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (!IsUiThread)
                InvokeOnUiAsync(action).Wait();
            else
                action();
        }
        #endregion

        #endregion
        #region internal

        #region EnsureUiThread
        /// <summary>
        /// Ensures that it's called from the Ui thread.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Must be called from the Ui thread.</exception>
        internal void EnsureUiThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != UiThread.ManagedThreadId)
                throw new InvalidOperationException("Must be called from the Ui thread.");
        }
        #endregion

        #endregion
        #region Protected

        #region OnShutdown
        /// <summary>
        /// Called when the application is shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            InvokeOnUi(() =>
            {
                FormsApplication.Exit();

                FormsApplication.ThreadException -= new ThreadExceptionEventHandler(Forms_UiUnhandledException);
                AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(Forms_UnhandledException);
            });
        }
        #endregion

        #endregion
        #region Private

        #region Forms_UnhandledException
        /// <summary>
        /// Handles the UnhandledException event of the Forms control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void Forms_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            Logger.Error("Unhandled exception.", exception);

            InvokeOnMainAsync(() =>
            {
                ShutdownWithException(exception);
            });
        }
        #endregion
        #region Forms_UiUnhandledException
        /// <summary>
        /// Handles the UiUnhandledException event of the Forms control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ThreadExceptionEventArgs"/> instance containing the event data.</param>
        private void Forms_UiUnhandledException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Error("Unhandled exception.", e.Exception);

            InvokeOnMainAsync(() =>
            {
                ShutdownWithException(e.Exception);
            });
        }
        #endregion

        #endregion
        #endregion
    }
}
