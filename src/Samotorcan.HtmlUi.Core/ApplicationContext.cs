using Samotorcan.HtmlUi.Core.Logs;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Application context.
    /// </summary>
    public class ApplicationContext : BaseApplicationSettings
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
        #region IncludeHtmUiScriptMapping
        /// <summary>
        /// Gets or sets a value indicating whether to include HTM UI script mapping.
        /// </summary>
        /// <value>
        /// <c>true</c> to include HTM UI script mapping; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeHtmUiScriptMapping { get; set; }
        #endregion
        #region LogSeverity
        /// <summary>
        /// Gets or sets the log severity.
        /// </summary>
        /// <value>
        /// The log severity.
        /// </value>
        public LogSeverity LogSeverity { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public ApplicationContext()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public ApplicationContext(ApplicationContext settings)
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
        private void InitializeSelf(ApplicationContext settings)
        {
            if (settings != null)
            {
                D3D11Enabled = settings.D3D11Enabled;
                CommandLineArgsEnabled = settings.CommandLineArgsEnabled;
                RemoteDebuggingPort = settings.RemoteDebuggingPort;
                ChromeViewsEnabled = settings.ChromeViewsEnabled;
                IncludeHtmUiScriptMapping = settings.IncludeHtmUiScriptMapping;
                LogSeverity = settings.LogSeverity;
            }
            else
            {
                D3D11Enabled = false;
                CommandLineArgsEnabled = false;
                RemoteDebuggingPort = null;
                ChromeViewsEnabled = false;
                IncludeHtmUiScriptMapping = false;
                LogSeverity = LogSeverity.Error;
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
        protected virtual void Initialize(ApplicationContext settings)
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
