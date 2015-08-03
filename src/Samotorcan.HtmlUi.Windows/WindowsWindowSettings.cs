using Samotorcan.HtmlUi.Core;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Windows window settings.
    /// </summary>
    public class WindowsWindowSettings : WindowsForms.WindowSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsWindowSettings"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowsWindowSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsWindowSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowsWindowSettings(WindowsWindowSettings settings)
        {
            Initialize(settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsWindowSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public WindowsWindowSettings(WindowSettings settings)
        {
            if (settings != null)
            {
                var windowsWindowSettings = settings as WindowsWindowSettings;

                if (windowsWindowSettings != null)
                {
                    Initialize(windowsWindowSettings);
                }
                else
                {
                    var windowsFormsWindowSettings = settings as WindowsForms.WindowSettings;

                    if (windowsFormsWindowSettings != null)
                        Initialize(windowsFormsWindowSettings);
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
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "settings", Justification = "Might have additional code in the future.")]
        private void InitializeSelf(WindowsWindowSettings settings)
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
        protected virtual void Initialize(WindowsWindowSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(settings);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected override void Initialize(WindowSettings settings)
        {
            base.Initialize(settings);

            InitializeSelf(null);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected override void Initialize(WindowsForms.WindowSettings settings)
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
