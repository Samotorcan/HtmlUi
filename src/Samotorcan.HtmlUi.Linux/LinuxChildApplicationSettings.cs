using Samotorcan.HtmlUi.Core;

namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Linux child application settings.
    /// </summary>
    public class LinuxChildApplicationSettings : WindowsForms.ChildApplicationSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxChildApplicationSettings"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public LinuxChildApplicationSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxChildApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public LinuxChildApplicationSettings(LinuxChildApplicationSettings settings)
        {
            Initialize(settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxChildApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public LinuxChildApplicationSettings(ChildApplicationSettings settings)
        {
            if (settings != null)
            {
                var linuxChildApplicationSettings = settings as LinuxChildApplicationSettings;

                if (linuxChildApplicationSettings != null)
                {
                    Initialize(linuxChildApplicationSettings);
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

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "settings", Justification = "Might have additional code in the future.")]
        private void InitializeSelf(LinuxChildApplicationSettings settings)
        {
            
        }

        #endregion
        #region Protected

        #region Initialize
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected virtual void Initialize(LinuxChildApplicationSettings settings)
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
        protected override void Initialize(ChildApplicationSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(null);
        }

        /// <summary>
        /// Initializes the specified settings.
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
