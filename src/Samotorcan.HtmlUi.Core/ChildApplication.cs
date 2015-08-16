using Samotorcan.HtmlUi.Core.Renderer;
using System;
using Xilium.CefGlue;
using Samotorcan.HtmlUi.Core.Utilities;
using Samotorcan.HtmlUi.Core.Messages;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Child application.
    /// </summary>
    public abstract class ChildApplication : BaseApplication
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
        public static new ChildApplication Current
        {
            get
            {
                return (ChildApplication)BaseApplication.Current;
            }
        }
        #endregion

        private CefBrowser CefBrowser { get; set; }

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplication"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.InvalidOperationException">Application must be run on child application process.</exception>
        protected ChildApplication(ChildApplicationSettings settings)
            : base(settings)
        {
            if (HtmlUiRuntime.ApplicationType != ApplicationType.ChildApplication)
                throw new InvalidOperationException("Application must be run on child application process.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplication"/> class with default settings.
        /// </summary>
        protected ChildApplication()
            : this(new ChildApplicationSettings()) { }

        #endregion
        #region Methods
        #region Protected

        #region RunInternal
        /// <summary>
        /// Run internal.
        /// </summary>
        protected override void RunInternal()
        {
            CefRuntime.Load();

            var mainArgs = new CefMainArgs(EnvironmentUtility.GetCommandLineArgs());

            var app = new App();
            app.BrowserCreated += (s, e) =>
            {
                CefBrowser = e.CefBrowser;
            };

            CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
        }
        #endregion

        protected override void SyncPropertyInternal(string name, object value)
        {
            MessageUtility.SendMessage(CefProcessId.Browser, CefBrowser, "syncProperty", new SyncProperty { Name = name, Value = value });
        }

        #endregion
        #endregion
    }
}
