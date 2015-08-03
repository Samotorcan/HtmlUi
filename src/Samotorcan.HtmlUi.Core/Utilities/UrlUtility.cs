using System;
using System.Text.RegularExpressions;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// Url utility,
    /// </summary>
    internal static class UrlUtility
    {
        #region Methods
        #region Public

        #region IsNativeRequestUrl
        /// <summary>
        /// Determines whether URL is native request.
        /// </summary>
        /// <param name="requestHostname">The request hostname.</param>
        /// <param name="nativeRequestPort">The native request port.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        public static bool IsNativeRequestUrl(string requestHostname, int nativeRequestPort, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            return Regex.IsMatch(url, string.Format("^(http://)?{0}:{1}(/.*)?$",
                    Regex.Escape(requestHostname),
                    nativeRequestPort),
                RegexOptions.IgnoreCase);
        }
        #endregion
        #region IsLocalUrl
        /// <summary>
        /// Determines whether URL is local request.
        /// </summary>
        /// <param name="requestHostname">The request hostname.</param>
        /// <param name="nativeRequestPort">The native request port.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        public static bool IsLocalUrl(string requestHostname, int nativeRequestPort, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            return Regex.IsMatch(url, string.Format("^(http://)?{0}((:80)?|:{1})(/.*)?$",
                    Regex.Escape(requestHostname),
                    nativeRequestPort),
                RegexOptions.IgnoreCase);
        }
        #endregion

        #endregion
        #endregion
    }
}
