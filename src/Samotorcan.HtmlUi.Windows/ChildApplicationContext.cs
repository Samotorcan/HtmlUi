using Samotorcan.HtmlUi.Core;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Windows child application context.
    /// </summary>
    public class ChildApplicationContext : WindowsForms.ChildApplicationContext
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplicationContext"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public ChildApplicationContext()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplicationContext"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public ChildApplicationContext(ChildApplicationContext settings)
        {
            Initialize(settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Using Initialize instead of a call to base constructor.")]
        public ChildApplicationContext(Core.ChildApplicationContext settings)
        {
            if (settings != null)
            {
                var windowsChildApplicationSettings = settings as ChildApplicationContext;

                if (windowsChildApplicationSettings != null)
                {
                    Initialize(windowsChildApplicationSettings);
                }
                else
                {
                    var windowsFormsChildApplicationSettings = settings as WindowsForms.ChildApplicationContext;

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
        private void InitializeSelf(ChildApplicationContext settings)
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
        protected virtual void Initialize(ChildApplicationContext settings)
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
        protected override void Initialize(Core.ChildApplicationContext settings)
        {
            base.Initialize(settings);

            InitializeSelf(null);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected override void Initialize(WindowsForms.ChildApplicationContext settings)
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
