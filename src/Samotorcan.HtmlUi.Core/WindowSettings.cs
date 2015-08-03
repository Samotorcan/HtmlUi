namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Window settings.
    /// </summary>
    public class WindowSettings
    {
        #region Properties
        #region Public

        #region View
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public string View { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowSettings"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowSettings(WindowSettings settings)
        {
            Initialize(settings);
        }

        #endregion
        #region Methods
        #region Private

        #region InitializeSelf
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void InitializeSelf(WindowSettings settings)
        {
            if (settings != null)
            {
                View = settings.View;
            }
            else
            {
                View = "/Views/Index.html";
            }
        }
        #endregion

        #endregion
        #region Protected

        #region Initialize
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected virtual void Initialize(WindowSettings settings)
        {
            InitializeSelf(settings);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
            Initialize(null);
        }
        #endregion

        #endregion
        #endregion
    }
}
