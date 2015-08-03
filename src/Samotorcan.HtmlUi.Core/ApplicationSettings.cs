namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public class ApplicationSettings : BaseApplicationSettings
    {
        #region Properties
        #region Public

        #region D3D11Enabled
        /// <summary>
        /// Gets a value indicating whether D3D11 is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if D3D11 is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool D3D11Enabled { get; set; }
        #endregion
        #region CommandLineArgsEnabled
        /// <summary>
        /// Gets or sets a value indicating whether command line arguments are enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if command line arguments are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CommandLineArgsEnabled { get; set; }
        #endregion
        #region RemoteDebuggingPort
        /// <summary>
        /// Gets or sets the remote debugging port. Set 0 to disable.
        /// </summary>
        /// <value>
        /// The remote debugging port.
        /// </value>
        public int? RemoteDebuggingPort { get; set; }
        #endregion
        #region ChromeViewsEnabled
        /// <summary>
        /// Gets or sets a value indicating whether chrome views are enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if chrome views are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool ChromeViewsEnabled { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettings"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public ApplicationSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public ApplicationSettings(ApplicationSettings settings)
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
        private void InitializeSelf(ApplicationSettings settings)
        {
            if (settings != null)
            {
                D3D11Enabled = settings.D3D11Enabled;
                CommandLineArgsEnabled = settings.CommandLineArgsEnabled;
                RemoteDebuggingPort = settings.RemoteDebuggingPort;
                ChromeViewsEnabled = settings.ChromeViewsEnabled;
            }
            else
            {
                D3D11Enabled = false;
                CommandLineArgsEnabled = false;
                RemoteDebuggingPort = null;
                ChromeViewsEnabled = false;
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
        protected virtual void Initialize(ApplicationSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(settings);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected override void Initialize(BaseApplicationSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(null);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            Initialize(null);
        }
        #endregion

        #endregion
        #endregion
    }
}
