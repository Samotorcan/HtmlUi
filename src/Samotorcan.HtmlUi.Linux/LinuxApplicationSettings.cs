using Samotorcan.HtmlUi.Core;

namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Linux application settings.
    /// </summary>
    public class LinuxApplicationSettings : WindowsForms.ApplicationSettings
    {
        #region Properties
        #region Public

        #region WindowSettings
        /// <summary>
        /// Gets the window settings.
        /// </summary>
        /// <value>
        /// The window settings.
        /// </value>
        public new LinuxWindowSettings WindowSettings
        {
            get
            {
                return (LinuxWindowSettings)base.WindowSettings;
            }
            protected set
            {
                base.WindowSettings = value;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxApplicationSettings"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public LinuxApplicationSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public LinuxApplicationSettings(LinuxApplicationSettings settings)
        {
            Initialize(settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public LinuxApplicationSettings(ApplicationSettings settings)
        {
            if (settings != null)
            {
                var linuxApplicationSettings = settings as LinuxApplicationSettings;

                if (linuxApplicationSettings != null)
                {
                    Initialize(linuxApplicationSettings);
                }
                else
                {
                    var windowsFormsApplicationSettings = settings as WindowsForms.ApplicationSettings;

                    if (windowsFormsApplicationSettings != null)
                        Initialize(windowsFormsApplicationSettings);
                    else
                        Initialize(settings);
                }
            }
            else
            {
                Initialize();
            }
        }

        #endregion
        #region Methods
        #region Private

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void InitializeSelf(LinuxApplicationSettings settings)
        {
            if (settings != null)
            {
                WindowSettings = settings.WindowSettings;
            }
            else
            {
                WindowSettings = new LinuxWindowSettings();
            }
        }

        #endregion
        #region Protected

        #region Initialize
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected virtual void Initialize(LinuxApplicationSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(settings);
        }

        /// <summary>
        /// Initializes the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected override void Initialize(BaseApplicationSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(null);
        }

        /// <summary>
        /// Initializes the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected override void Initialize(ApplicationSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(null);
        }

        /// <summary>
        /// Initializes the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected override void Initialize(WindowsForms.ApplicationSettings settings)
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
