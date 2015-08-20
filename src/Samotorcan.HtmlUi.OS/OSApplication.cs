using System;
using Samotorcan.HtmlUi.Core;

namespace Samotorcan.HtmlUi.OS
{
    /// <summary>
    /// OS application.
    /// </summary>
    public static class OSApplication
    {
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Run(ApplicationContext settings)
        {
            switch (HtmlUiRuntime.Platform)
            {
                case Platform.Windows:
                    Windows.RunApplication(settings);
                    break;
                case Platform.Linux:
                    Linux.RunApplication(settings);
                    break;
                case Platform.OSX:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        public static void Run()
        {
            Run(null);
        }
        #endregion

        #endregion
        #endregion
    }
}
