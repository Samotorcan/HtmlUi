using System;

namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Linux application.
    /// </summary>
    [CLSCompliant(false)]
    public class LinuxApplication : WindowsForms.Application
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
        public static new LinuxApplication Current
        {
            get
            {
                return (LinuxApplication)WindowsForms.Application.Current;
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
        public new LinuxWindow Window
        {
            get
            {
                return (LinuxWindow)base.Window;
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
        protected new LinuxApplicationSettings Settings
        {
            get
            {
                return (LinuxApplicationSettings)base.Settings;
            }
            set
            {
                base.Settings = value;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxApplication"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public LinuxApplication(LinuxApplicationSettings settings)
            : base(settings) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxApplication"/> class.
        /// </summary>
        public LinuxApplication()
            : this(new LinuxApplicationSettings()) { }

        #endregion
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Run(LinuxApplicationSettings settings)
        {
            if (settings == null)
                settings = new LinuxApplicationSettings();

            using (var application = new LinuxApplication(settings))
            {
                application.RunApplication();
            }
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        public static void Run()
        {
            Run(null);
        }
        #endregion

        #endregion
        #region Protected

        #region OnInitialize
        /// <summary>
        /// Called when the application is initialized.
        /// </summary>
        protected override void OnInitialize()
        {
            InvokeOnUiAsync(() =>
            {
                Window = new LinuxWindow(Settings.WindowSettings);
                Window.Form.Show();
            });
        }
        #endregion

        #endregion
        #endregion
    }
}
