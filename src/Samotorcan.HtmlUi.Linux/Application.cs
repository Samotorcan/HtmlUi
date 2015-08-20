using System;

namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Linux application.
    /// </summary>
    [CLSCompliant(false)]
    public class Application : WindowsForms.Application
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
                return (Application)WindowsForms.Application.Current;
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
        protected new ApplicationContext Settings
        {
            get
            {
                return (ApplicationContext)base.Settings;
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
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public Application(ApplicationContext settings)
            : base(settings) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        public Application()
            : this(new ApplicationContext()) { }

        #endregion
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Run(ApplicationContext settings)
        {
            if (settings == null)
                settings = new ApplicationContext();

            using (var application = new Application(settings))
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
                Window = new Window(Settings.WindowSettings);
                Window.Form.Show();
            });
        }
        #endregion

        #endregion
        #endregion
    }
}
