using Samotorcan.HtmlUi.Core;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Windows application settings.
    /// </summary>
    public class WindowsApplicationSettings : WindowsForms.ApplicationSettings
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
        public new WindowsWindowSettings WindowSettings
        {
            get
            {
                return (WindowsWindowSettings)base.WindowSettings;
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
        /// Initializes a new instance of the <see cref="WindowsApplicationSettings"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowsApplicationSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowsApplicationSettings(WindowsApplicationSettings settings)
        {
            Initialize(settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowsApplicationSettings(ApplicationSettings settings)
        {
            if (settings != null)
            {
                var windowsApplicationSettings = settings as WindowsApplicationSettings;

                if (windowsApplicationSettings != null)
                {
                    Initialize(windowsApplicationSettings);
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

        #region InitializeSelf
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void InitializeSelf(WindowsApplicationSettings settings)
        {
            if (settings != null)
            {
                WindowSettings = settings.WindowSettings;
            }
            else
            {
                WindowSettings = new WindowsWindowSettings(base.WindowSettings);
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
        protected virtual void Initialize(WindowsApplicationSettings settings)
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
        /// <param name="settings">The settings.</param>
        protected override void Initialize(ApplicationSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(null);
        }

        /// <summary>
        /// Initializes this instance.
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
