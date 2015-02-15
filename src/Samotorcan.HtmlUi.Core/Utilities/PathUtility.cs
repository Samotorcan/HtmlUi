using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// Path utility.
    /// </summary>
    public static class PathUtility
    {
        #region Properties
        #region Public

        #region WorkingDirectory
        private static string _workingDirectory;
        /// <summary>
        /// Gets the working directory.
        /// </summary>
        /// <value>
        /// The working directory.
        /// </value>
        public static string WorkingDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_workingDirectory))
                    _workingDirectory = Path.GetDirectoryName(PathUtility.Application);

                return _workingDirectory;
            }
        }
        #endregion
        #region Application
        private static string _application;
        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        public static string Application
        {
            get
            {
                if (string.IsNullOrEmpty(_application))
                    _application = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;

                return _application;
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
