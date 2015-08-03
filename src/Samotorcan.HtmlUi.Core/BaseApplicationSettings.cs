namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Base application settings.
    /// </summary>
    public class BaseApplicationSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApplicationSettings"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public BaseApplicationSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApplicationSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public BaseApplicationSettings(BaseApplicationSettings settings)
        {
            Initialize(settings);
        }

        #endregion
        #region Methods
        #region Private

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "settings", Justification = "Might have additional code in the future.")]
        private void InitializeSelf(BaseApplicationSettings settings)
        {

        }

        #endregion
        #region Protected

        #region Initialize
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected virtual void Initialize(BaseApplicationSettings settings)
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
