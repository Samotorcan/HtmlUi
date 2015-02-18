using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        #region NormalizedWorkingDirectory
        private static string _normalizedWorkingDirectory;
        /// <summary>
        /// Gets the normalized working directory.
        /// </summary>
        /// <value>
        /// The normalized working directory.
        /// </value>
        public static string NormalizedWorkingDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_normalizedWorkingDirectory))
                    _normalizedWorkingDirectory = PathUtility.WorkingDirectory.Replace('\\', '/');

                return _normalizedWorkingDirectory;
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
        #region Methods
        #region Public

        #region IsFileName
        /// <summary>
        /// Determines whether file name is valid.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static bool IsFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            return Regex.IsMatch(fileName, @"^[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)?$");
        }
        #endregion
        #region IsFilePath
        /// <summary>
        /// Determines whether file path is valid.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static bool IsFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            return Regex.IsMatch(filePath, @"^/?([a-zA-Z0-9]+/?)+$");
        }
        #endregion
        #region IsFullFileName
        /// <summary>
        /// Determines whether full file name is valid.
        /// </summary>
        /// <param name="fullFileName">Full name of the file.</param>
        /// <returns></returns>
        public static bool IsFullFileName(string fullFileName)
        {
            if (string.IsNullOrWhiteSpace(fullFileName))
                return false;

            var fileParts = fullFileName.Split('/');
            var fileName = fileParts.Last();
            var filePath = string.Join(string.Empty, fileParts.Take(fileParts.Length - 1));

            return PathUtility.IsFileName(fileName) && (string.IsNullOrEmpty(filePath) || PathUtility.IsFilePath(filePath));
        }
        #endregion

        #endregion
        #endregion
    }
}
