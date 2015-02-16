using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormsApplication = System.Windows.Forms.Application;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Application.
    /// </summary>
    [CLSCompliant(false)]
    public class Application : Core.Application
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

        #endregion
        #region Internal

        #region Window
        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        internal new Window Window
        {
            get
            {
                return (Window)base.Window;
            }
        }
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
        #region UiSynchronizationContext
        /// <summary>
        /// Gets or sets the UI synchronization context.
        /// </summary>
        /// <value>
        /// The UI synchronization context.
        /// </value>
        private WindowsFormsSynchronizationContext UiSynchronizationContext { get; set; }
        #endregion
        #region UiSynchronizationContextCreated
        /// <summary>
        /// Gets or sets the UI synchronization context created event.
        /// </summary>
        /// <value>
        /// The UI synchronization context created event.
        /// </value>
        private AutoResetEvent UiSynchronizationContextCreated { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        public Application()
            : base()
        {
            UiSynchronizationContextCreated = new AutoResetEvent(false);

            UiThread = new Thread(UiThreadStart);
            UiThread.SetApartmentState(ApartmentState.STA);
            UiThread.Start();

            UiSynchronizationContextCreated.WaitOne();

            InitializeWindow();
        }

        #endregion
        #region Methods
        #region Public

        #region InvokeOnUi
        /// <summary>
        /// Invokes the specified action on the UI thread.
        /// </summary>
        /// <param name="synchronous">if set to <c>true</c> invoke synchronous .</param>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        internal void InvokeOnUi(bool synchronous, Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (synchronous)
                UiSynchronizationContext.Send((state) => { action(); }, null);
            else
                UiSynchronizationContext.Post((state) => { action(); }, null);
        }

        /// <summary>
        /// Invokes the specified action on the UI thread.
        /// </summary>
        /// <param name="action">The action.</param>
        public void InvokeOnUi(Action action)
        {
            InvokeOnUi(false, action);
        }
        #endregion

        #endregion
        #region internal

        #region EnsureUiThread
        /// <summary>
        /// Ensures that it's called from the UI thread.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Must be called from the main thread.</exception>
        internal void EnsureUiThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != UiThread.ManagedThreadId)
                throw new InvalidOperationException("Must be called from the main thread.");
        }
        #endregion

        #endregion
        #region Protected

        #region Initialize
        /// <summary>
        /// Called when the application is initialized.
        /// </summary>
        protected override void Initialize()
        {
            InvokeOnUi(() =>
            {
                Window.Form.Show();
            });
        }
        #endregion
        #region OnShutdown
        /// <summary>
        /// Called when the application is shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            InvokeOnUi(true, () =>
            {
                FormsApplication.Exit();
            });
        }
        #endregion
        #region CreateWindow
        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <returns></returns>
        protected override Core.Window CreateWindow()
        {
            return new Window();
        }
        #endregion

        #endregion
        #region Private

        #region UiThreadStart
        /// <summary>
        /// UI thread start.
        /// </summary>
        private void UiThreadStart()
        {
            FormsApplication.EnableVisualStyles();
            FormsApplication.SetCompatibleTextRenderingDefault(false);
            
            SynchronizationContext.SetSynchronizationContext(UiSynchronizationContext = new WindowsFormsSynchronizationContext());
            UiSynchronizationContextCreated.Set();

            FormsApplication.Run();
        }
        #endregion

        #endregion
        #endregion
    }
}
