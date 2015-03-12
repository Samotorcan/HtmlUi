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

        #region Normalize
        /// <summary>
        /// Normalizes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="normalizeType">The normalize type.</param>
        /// <returns></returns>
        public static string Normalize(string value, NormalizeType normalizeType)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (normalizeType == NormalizeType.CamelCase)
                return Char.ToLowerInvariant(value[0]) + value.Substring(1);
            else if (normalizeType == NormalizeType.PascalCase)
                return Char.ToUpperInvariant(value[0]) + value.Substring(1);

            return value;
        }
        #endregion

        #endregion
        #endregion
    }
}
