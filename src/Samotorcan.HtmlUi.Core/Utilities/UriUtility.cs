using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// Uri utility.
    /// </summary>
    public static class UriUtility
    {
        #region Methods
        #region Public

        #region IsAbsoluteUri
        /// <summary>
        /// Determines whether value is absolute URI.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool IsAbsoluteUri(string value)
        {
            Uri uri;

            return Uri.TryCreate(value, UriKind.Absolute, out uri);
        }
        #endregion

        #endregion
        #endregion
    }
}
