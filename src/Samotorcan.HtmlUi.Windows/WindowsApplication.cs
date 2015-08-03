using System;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Windows application.
    /// </summary>
    [CLSCompliant(false)]
    public class WindowsApplication : WindowsForms.Application
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
        public static new WindowsApplication Current
        {
            get
            {
                return (WindowsApplication)WindowsForms.Application.Current;
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
        public new WindowsWindow Window
        {
            get
            {
                return (WindowsWindow)base.Window;
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
        protected new WindowsApplicationSettings Settings
        {
            get
            {
                return (WindowsApplicationSettings)base.Settings;
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
        /// Initializes a new instance of the <see cref="WindowsApplication"/> class.
        /// </summary>
        internal WindowsApplication(WindowsApplicationSettings settings)
            : base(settings) { }

        #endregion
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Run(WindowsApplicationSettings settings)
        {
            if (settings == null)
                settings = new WindowsApplicationSettings();

            using (var application = new WindowsApplication(settings))
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
                Window = new WindowsWindow(Settings.WindowSettings);
                Window.Form.Show();
            });
        }
        #endregion

        #endregion
        #endregion
    }
}
