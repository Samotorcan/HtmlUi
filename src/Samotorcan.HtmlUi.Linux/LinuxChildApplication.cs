namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Linux child application.
    /// </summary>
    public class LinuxChildApplication : WindowsForms.ChildApplication
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
        public static new LinuxChildApplication Current
        {
            get
            {
                return (LinuxChildApplication)WindowsForms.ChildApplication.Current;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxChildApplication"/> class.
        /// </summary>
        public LinuxChildApplication(LinuxChildApplicationSettings settings)
            : base(settings) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxChildApplication"/> class with default settings.
        /// </summary>
        public LinuxChildApplication()
            : this(new LinuxChildApplicationSettings()) { }

        #endregion
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Run(LinuxChildApplicationSettings settings)
        {
            if (settings == null)
                settings = new LinuxChildApplicationSettings();

            using (var application = new LinuxChildApplication(settings))
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
