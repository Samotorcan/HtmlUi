namespace Samotorcan.HtmlUi.WindowsForms
{
    /// <summary>
    /// Windows forms child application.
    /// </summary>
    public abstract class ChildApplication : Core.ChildApplication
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
                return (ChildApplication)Core.ChildApplication.Current;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplication"/> class.
        /// </summary>
        protected ChildApplication(ChildApplicationContext settings)
            : base(settings) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplication"/> class with default settings.
        /// </summary>
        protected ChildApplication()
            : this(new ChildApplicationContext()) { }

        #endregion
    }
}
