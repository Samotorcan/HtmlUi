using System;
using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Linux;
using Samotorcan.HtmlUi.Windows;

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
        public static void Run(ApplicationSettings settings)
        {
            switch (HtmlUiRuntime.Platform)
            {
                case Platform.Windows:
                    WindowsApplication.Run(new WindowsApplicationSettings(settings));
                    break;
                case Platform.Linux:
                    LinuxApplication.Run(new LinuxApplicationSettings(settings));
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
