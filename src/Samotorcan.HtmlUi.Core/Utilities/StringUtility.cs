using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// String utility.
    /// </summary>
    internal static class StringUtility
    {
        #region Methods
        #region Public

        #region CamelCase
        /// <summary>
        /// Camel case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string CamelCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return Char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
        #endregion

        #endregion
        #endregion
    }
}
