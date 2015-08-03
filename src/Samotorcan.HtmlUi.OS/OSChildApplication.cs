using System;
using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Linux;
using Samotorcan.HtmlUi.Windows;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.OS
{
    /// <summary>
    /// OS child application.
    /// </summary>
    public static class OSChildApplication
    {
        #region Methods
        #region Public

        #region Run
        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public static void Run(ChildApplicationSettings settings)
        {
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    WindowsChildApplication.Run(new WindowsChildApplicationSettings(settings));
                    break;
                case CefRuntimePlatform.Linux:
                    LinuxChildApplication.Run(new LinuxChildApplicationSettings(settings));
                    break;
                case CefRuntimePlatform.MacOSX:
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
