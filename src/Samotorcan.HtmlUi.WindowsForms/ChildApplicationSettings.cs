using Samotorcan.HtmlUi.Core;

namespace Samotorcan.HtmlUi.WindowsForms
{
    /// <summary>
    /// Child application settings.
    /// </summary>
    public class ChildApplicationSettings : Core.ChildApplicationSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplicationSettings"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public ChildApplicationSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public ChildApplicationSettings(ChildApplicationSettings settings)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "settings", Justification = "Might have additional code in the future.")]
        private void InitializeSelf(ChildApplicationSettings settings)
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
        protected virtual void Initialize(ChildApplicationSettings settings)
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
        protected override void Initialize(Core.ChildApplicationSettings settings)
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
