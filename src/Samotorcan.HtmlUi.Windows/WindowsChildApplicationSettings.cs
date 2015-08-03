using Samotorcan.HtmlUi.Core;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Windows child application settings.
    /// </summary>
    public class WindowsChildApplicationSettings : WindowsForms.ChildApplicationSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsChildApplicationSettings"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowsChildApplicationSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsChildApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowsChildApplicationSettings(WindowsChildApplicationSettings settings)
        {
            Initialize(settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowsChildApplicationSettings(ChildApplicationSettings settings)
        {
            if (settings != null)
            {
                var windowsChildApplicationSettings = settings as WindowsChildApplicationSettings;

                if (windowsChildApplicationSettings != null)
                {
                    Initialize(windowsChildApplicationSettings);
                }
                else
                {
                    var windowsFormsChildApplicationSettings = settings as WindowsForms.ChildApplicationSettings;

                    if (windowsFormsChildApplicationSettings != null)
                        Initialize(windowsFormsChildApplicationSettings);
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "settings", Justification = "Might have additional code in the future.")]
        private void InitializeSelf(WindowsChildApplicationSettings settings)
        {
            
        }
        #endregion

        #endregion
        #region Protected

        #region Initialize
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected virtual void Initialize(WindowsChildApplicationSettings settings)
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
        protected override void Initialize(ChildApplicationSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(null);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected override void Initialize(WindowsForms.ChildApplicationSettings settings)
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
