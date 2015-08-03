namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Windows child application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1052:StaticHolderTypesShouldBeSealed", Justification = "It might not contain only static methods in the future.")]
    public class WindowsChildApplication : WindowsForms.ChildApplication
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
        public static new WindowsChildApplication Current
        {
            get
            {
                return (WindowsChildApplication)WindowsForms.ChildApplication.Current;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsChildApplication"/> class.
        /// </summary>
        private WindowsChildApplication(WindowsChildApplicationSettings settings)
            : base(settings) { }

        #endregion
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Run(WindowsChildApplicationSettings settings)
        {
            if (settings == null)
                settings = new WindowsChildApplicationSettings();

            using (var application = new WindowsChildApplication(settings))
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
        #endregion
    }
}
