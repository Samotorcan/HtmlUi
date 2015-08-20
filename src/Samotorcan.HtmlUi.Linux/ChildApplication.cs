namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Linux child application.
    /// </summary>
    public class ChildApplication : WindowsForms.ChildApplication
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
                return (ChildApplication)WindowsForms.ChildApplication.Current;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplication"/> class.
        /// </summary>
        public ChildApplication(ChildApplicationSettings settings)
            : base(settings) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplication"/> class with default settings.
        /// </summary>
        public ChildApplication()
            : this(new ChildApplicationSettings()) { }

        #endregion
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Run(ChildApplicationSettings settings)
        {
            if (settings == null)
                settings = new ChildApplicationSettings();

            using (var application = new ChildApplication(settings))
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
