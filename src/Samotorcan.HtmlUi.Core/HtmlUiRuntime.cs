using System.Linq;
using Samotorcan.HtmlUi.Core.Utilities;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Html UI runtime.
    /// </summary>
    public static class HtmlUiRuntime
    {
        #region Properties
        #region Public

        #region ApplicationType
        private static ApplicationType? _applicationType;
        /// <summary>
        /// Gets the type of the application.
        /// </summary>
        /// <value>
        /// The type of the application.
        /// </value>
        public static ApplicationType ApplicationType
        {
            get
            {
                if (_applicationType == null)
                {
                    var processArguments = EnvironmentUtility.GetCommandLineArgs();

                    _applicationType = processArguments.Any(a => a.StartsWith("--type=")) ? ApplicationType.ChildApplication : ApplicationType.Application;
                }

                return _applicationType.Value;
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
