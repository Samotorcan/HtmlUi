using System;
using Samotorcan.HtmlUi.Core;

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
        public static void Run(ChildApplicationContext settings)
        {
            switch (HtmlUiRuntime.Platform)
            {
                case Platform.Windows:
                    Windows.RunChildApplication(settings);
                    break;
                case Platform.Linux:
                    Linux.RunChildApplication(settings);
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
