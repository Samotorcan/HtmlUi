using System;
using System.Linq;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// EnvironmentUtility
    /// </summary>
    internal static class EnvironmentUtility
    {
        #region Methods
        #region Public

        #region GetCommandLineArgs
        /// <summary>
        /// Gets the command line arguments.
        /// </summary>
        /// <returns></returns>
        public static string[] GetCommandLineArgs()
        {
            var args = Environment.GetCommandLineArgs().AsEnumerable();

            if (CefRuntime.Platform == CefRuntimePlatform.Windows)
                args = args.Skip(1);

            return args.ToArray();
        }
        #endregion

        #endregion
        #endregion
    }
}
